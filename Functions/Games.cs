using DiscordRPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LIFTOFF.Functions
{
    public class Game
    {
        public override string ToString()
        {
            return Title;
        }

        //"{GameIcon},{GameTitle},{AppID},{ImgURL},{LinkURL},{Filename},{ServerFilename},{PlainName},{ServerAppID}"
        public string GameIcon { get; set; }
        public string Title { get; set; }
        public uint AppID { get; set; }

        public ImageSource Banner
        {
            get
            {
                ImageBrush bannerImg;
                if (cachedBanner == null)
                {
                    try
                    {
                        bannerImg = Games.ImgBrushFromURL(BannerURL).Result;
                        cachedBanner = bannerImg;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Error Getting Banner!");
                        bannerImg = Games.ImgBrushFromURL(DefaultBannerURL).Result;
                    }
                }
                else
                {
                    bannerImg = cachedBanner;
                }

                return bannerImg.ImageSource;
            }
        }
        private ImageBrush cachedBanner { get; set; }

        public string BannerURL
        {
            get
            {
                if (CustomBannerURL != "") return CustomBannerURL;
                else return DefaultBannerURL;
            }
            set 
            {
                CustomBannerURL = value;
            }
        }

        private string CustomBannerURL { get; set; }
        private string DefaultBannerURL
        {
            get
            {
                {
                    return "https://cdn.cloudflare.steamstatic.com/steam/apps/" + AppID.ToString() + "/header.jpg";
                }
            }
        } 
        public string LinkURL { get; set; }
        public string FileLocation { get; set; }
        public string ServerFilename { get; set; }
        public string Plainname { get; set; }
        public string AdditionalArguments { get; set; }
        public uint ServerAppID { get; set; }
    }

    class Games
    {

        public static async Task<ImageBrush> ImgBrushFromURL(string URL = null)
        {
            ImageBrush ImgBrush = new ImageBrush();
            if (URL != "")
            {
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Getting image from url (" + URL + ")");

                WebRequest ImgRequest = WebRequest.Create(URL);

                WebResponse ImgResponse = ImgRequest.GetResponse();

                Stream ImgStream = ImgResponse.GetResponseStream();

                BitmapImage ImgBitmap = new BitmapImage();
                ImgBitmap.BeginInit();
                ImgBitmap.StreamSource = ImgStream;
                ImgBitmap.EndInit();


                ImgBrush.ImageSource = ImgBitmap;
            }
            return ImgBrush;
        }

        public static async Task<bool> IsGameRunning()
        {
            Process[] processes = Process.GetProcessesByName(Variables.CurrentGame.Plainname);
            if (processes.Length > 0) return true;
            else return false;
        }

        public static async Task AddGame(Functions.Game game)
        {
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Adding Game... (" + game.Title + " - " + game.AppID + ")");
            Game pg = new Game();
            Random r = new Random();

            pg.GameIcon = null;
            pg.Title = game.Title;
            pg.AppID = game.AppID;
           
            pg.BannerURL = game.BannerURL;
            pg.FileLocation = game.FileLocation;
            pg.LinkURL = null;
            pg.Plainname = game.FileLocation;
            pg.AdditionalArguments = game.AdditionalArguments;

            Variables.GameList.Add(pg);
            await SetLocalGames();
        }

        public static async Task JoinGame(Functions.Server currServer)
        {
            Variables.GameRunning = true;
            try
            {
                Process rocket = new Process();

                rocket.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\ROCKET.exe";
                rocket.StartInfo.Arguments = "#join " + currServer.IpandPort() + " " + currServer.Game.AppID + " " + Path.GetFileName(currServer.Game.FileLocation + " " + Variables.CurrentGame.AdditionalArguments);

                if (!Windows.Preloader.debug)
                {
                    rocket.StartInfo.RedirectStandardOutput = true;
                    rocket.StartInfo.RedirectStandardError = true;
                    rocket.EnableRaisingEvents = true;
                    rocket.StartInfo.UseShellExecute = false;
                    rocket.StartInfo.CreateNoWindow = true;
                }
                
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Starting NamedPipe Server...");
                NamedPipeServerStream server = new NamedPipeServerStream("LIFTOFF");

                await Task.Delay(4000);
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Starting ROCKET...");
                rocket.Start();

                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Waiting for message from ROCKET...");
                server.WaitForConnection();

                StreamReader reader = new StreamReader(server);

                string response = reader.ReadLine();
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Got response from ROCKET (" + response + ")");
                if (!bool.Parse(response.Split(",")[0]))
                {
                    string ex = response.Split(",")[1];
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Error launching game - " + ex);
                    MessageBox.Show("ERROR LAUNCHING GAME!" + Environment.NewLine +  ex);
                }
                else
                {
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Game launched!");
                }

                await rocket.WaitForExitAsync();
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Game closed.");
                Variables.GameRunning = false;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
                Variables.GameRunning = false;
            }
        }


        public static async Task GetLocalGames()
        {
            if (File.Exists(Variables.ConfigDir + @"\mygames.json"))
            {
                List<Game> pgList = new List<Game>();

                string json = File.ReadAllText(Variables.ConfigDir + @"\mygames.json");
                if (json != "") pgList = JsonConvert.DeserializeObject<List<Game>>(json);
                Variables.GameList = pgList;
            }
        }

        public static async Task SetLocalGames()
        {
            List<Game> pgList = new List<Game>();

            foreach (Game g in Variables.GameList) pgList.Add(g);

            File.WriteAllText(Variables.ConfigDir + @"\mygames.json", JsonConvert.SerializeObject(pgList, Formatting.Indented));
        }
    }
}
