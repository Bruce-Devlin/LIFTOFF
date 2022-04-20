using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LIFTOFF.Functions
{

    public class Server
    {
        public string TotalPlayers { get; set; }
        public string IpandPort() { return Info.addr + ":" + Info.gameport; }
        public SteamServer Info { get; set; }
        public bool Modded  { get; set; }
        public bool Featured { get; set; }
        public List<Mod> Mods { get; set; }
        public Game Game { get; set; }
    }

    public class Mod
    {
        public uint ItemID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
    }
    
    public class SteamServer
    {
        public string addr { get; set; }
        public int gameport { get; set; }
        public ulong steamid { get; set; }
        public string name { get; set; }
        public uint appid { get; set; }
        public string gamedir { get; set; }
        public string version { get; set; }
        public string product { get; set; }
        public int region { get; set; }
        public int players { get; set; }
        public int max_players { get; set; }
        public int bots { get; set; }
        public string map { get; set; }
        public bool secure { get; set; }
        public bool dedicated { get; set; }
        public string os { get; set; }
        public string gametype { get; set; }
    }
    internal class Servers
    {
        public static async Task<ObservableCollection<Server>> GetServers(int count, Windows.Home homeWin, string search = "")
        {
            ObservableCollection<Server> ServerList = new ObservableCollection<Server>();
            try
            {
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Getting servers from Steam for app: " + Variables.CurrentGame.AppID);
                System.Diagnostics.Stopwatch requestTime = System.Diagnostics.Stopwatch.StartNew();

                JObject result;
                
                ServicePointManager.DefaultConnectionLimit = 4;
                ServicePointManager.Expect100Continue = false;
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Sending request to API...");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://liftoff.publiczeus.com/api/ServerList?appID=" + Variables.CurrentGame.AppID);
                request.Timeout = 10000;
                request.Proxy = null;
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Got response from API!");

                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Reading Stream from API...");
                    result = JObject.Parse(await reader.ReadToEndAsync());
                    reader.Close();
                    stream.Close();
                    response.Close();

                    requestTime.Stop();
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Done reading from Stream. (" + requestTime.ElapsedMilliseconds + "ms)");
                }

                System.Diagnostics.Stopwatch compileTime = System.Diagnostics.Stopwatch.StartNew();
                if (int.Parse(result["code"].ToString()) != 400)
                {
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Compiling response from API...");

                    int servers = 0;
                    int totalPlayers = 0;
                    string[] featuredIPs;

                    using (var client = new WebClient())
                    {
                        var content = client.DownloadData("https://liftoff.publiczeus.com/featured.txt");
                        using (var stream = new MemoryStream(content))
                        {
                            StreamReader sr = new StreamReader(stream);
                            featuredIPs = sr.ReadToEnd().Split(",");
                        }
                    }

                    foreach (JObject server in result["data"])
                    {
                        if (servers != count)
                        {
                            SteamServer newSteamServer = server.ToObject<SteamServer>();

                            if (newSteamServer.map == null) newSteamServer.map = "";
                            if (search == "" || newSteamServer.name.ToUpper().Contains(search.ToUpper()) || newSteamServer.addr.ToUpper().Contains(search.ToUpper()))
                            {
                                Server newServer = new Server();

                                var ipWithoutPort = newSteamServer.addr.Split(':');
                                newSteamServer.addr = ipWithoutPort[0];

                                newServer.Info = newSteamServer;
                                newServer.Game = Variables.CurrentGame;
                                newServer.TotalPlayers = newServer.Info.players + "/" + newServer.Info.max_players;
                                totalPlayers += newServer.Info.players;

                                if (newSteamServer.gametype != null)
                                {
                                    if (newSteamServer.gametype.Contains("mod"))
                                    {
                                        newServer.Modded = true;
                                        //Get Server Mods
                                    }
                                    else newServer.Modded = false;
                                }

                                if (featuredIPs.Contains(newSteamServer.addr + ":" + newSteamServer.gameport))
                                {
                                    newServer.Featured = true;
                                }
                                else newServer.Featured = false;

                                ServerList.Add(newServer);
                                servers++;
                            }
                        }
                        else break;
                    };
                    compileTime.Stop();
                    homeWin.Counter.Text = totalPlayers.ToString("N0") + " PLAYERS | " + servers.ToString("N0") + " SERVERS";

                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Got " + servers + " servers! (response-time: " + requestTime.ElapsedMilliseconds +"ms | compile-time: " + compileTime.ElapsedMilliseconds + "ms)");
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Total players found: " + totalPlayers);
                }
                else MessageBox.Show("Uh-oh!? It looks like I cant connect to steam servers... Maybe Steam is down right now? or maybe check you are connected to the internet and restart me.");
            }
            catch (Exception Ex)
            {
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Error getting servers from Steam!");
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: " + Ex.ToString());
            }
            return ServerList;
        }
    }
}
