using DiscordRPC;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
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
using System.Net.Http;
using System.Drawing;
using System.Windows.Interop;

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
        public string Title 
        { 
            get
            {
                string title = OGTitle;
                if (Featured)
                {
                    title = "❤️ " + title;
                }
                return title;
            }
            set
            {
                OGTitle = value;
            }
        }
        private string OGTitle { get; set; }
        public uint AppID { get; set; }
        public bool Featured { get; set; }
        public ImageSource Banner
        {
            get
            {
                Core.Log("Getting banner Image for " + AppID + "...");
                ImageBrush bannerImg;
                if (cachedBanner == null)
                {
                    bannerImg = Games.ImgBrushFromURL(BannerURL).Result;
                    cachedBanner = bannerImg;

                    if (bannerImg == null)
                    {
                        Core.Log("No custom banner, getting the default instead for " + AppID + ".");
                        bannerImg = Games.ImgBrushFromURL(DefaultBannerURL).Result;
                        cachedBanner = bannerImg;
                    }
                }
                else
                {
                    Core.Log("Got cached banner for " + AppID + ".");
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
        public List<string> FileLocations { get; set; }
        public string Filename { get; set; }
        private string ServerFilename { get; set; }
        public string Plainname { get; set; }
        public string AdditionalArguments { get; set; }
        private uint ServerAppID { get; set; }
    }

    public class SteamLibrary
    {
        public int id { get; set; }
    }

    class Games
    {
        public static async Task<bool> IsGameInstalled(ulong appID)
        {
            string SteamLibsFileLoc = "";

            if (Functions.Core.getVariable("SteamLibsFile") == "")
            {
                if (!File.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\libraryfolders.vdf"))
                {
                    await Functions.Core.Log("Cant find library file, please select a new one");
                    OpenFileDialog fileDialog = new OpenFileDialog
                    {
                        Title = "Select \"libraryfolder.vdf\"",

                        CheckFileExists = true,
                        CheckPathExists = true,

                        DefaultExt = ".vdf",
                        Filter = "VDF files|*.vdf",
                        FilterIndex = 2,
                        RestoreDirectory = true,

                        ReadOnlyChecked = true,
                        ShowReadOnly = true
                    };

                    if ((bool)fileDialog.ShowDialog())
                    {
                        SteamLibsFileLoc = fileDialog.FileName;
                    }
                }
                else SteamLibsFileLoc = "C:\\Program Files (x86)\\Steam\\steamapps\\libraryfolders.vdf";
                Functions.Core.storeVariable("SteamLibsFile", SteamLibsFileLoc);
            }

            bool installed = false;

            if (File.ReadAllText(Functions.Core.getVariable("SteamLibsFile")).Contains(appID.ToString()))
                installed = true;

            await Core.Log("App " + appID + " installed = " + installed);

            return installed;
        }

        public static async Task<ImageBrush> ImgBrushFromURL(string URL = null)
        {
            if (URL != null)
            {
                ImageBrush ImgBrush = new ImageBrush();

                await Core.Log("Getting image from url (" + URL + ")");
                try
                {
                    await Core.Log("Sending HTTP request...");
                    var httpClient = new HttpClient();

                    Stream imageStream = httpClient.GetStreamAsync(URL).Result;
                    using (Bitmap bitmap = new Bitmap(Image.FromStream(imageStream)))
                    {
                        ImgBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                                                                            IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }
                catch
                {
                    await Core.Log("Failed to get banner :/", true);
                }
                
                return ImgBrush;
            }
            else return null;
        }

        public static async Task<bool> IsGameRunning()
        {
            Process[] processes = Process.GetProcessesByName(Variables.CurrentGame.Plainname);
            if (processes.Length > 0) return true;
            else return false;
        }

        public static async Task AddGame(Functions.Game game)
        {
            await Core.Log("Adding Game... (" + game.Title + " - " + game.AppID + ")");
            Game pg = new Game();
            Random r = new Random();

            pg.GameIcon = null;
            pg.Title = game.Title;
            pg.AppID = game.AppID;
           
            pg.BannerURL = game.BannerURL;
            //pg.FileLocation = game.FileLocation;
            //pg.Filename = Path.GetFileName(pg.FileLocation);
            pg.LinkURL = null;
            //pg.Plainname = game.FileLocation;
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
                rocket.StartInfo.Arguments = "#join " + currServer.IPandPort() + " " + currServer.Game.AppID + " " + currServer.Game.Filename + " " + Variables.CurrentGame.AdditionalArguments;
                rocket.StartInfo.Verb = "runas";

                if (!Windows.Preloader.debug)
                {
                    rocket.StartInfo.RedirectStandardOutput = true;
                    rocket.StartInfo.RedirectStandardError = true;
                    rocket.EnableRaisingEvents = true;
                    rocket.StartInfo.UseShellExecute = false;
                    rocket.StartInfo.CreateNoWindow = true;
                }
                
                await Core.Log("Starting NamedPipe Server...");
                NamedPipeServerStream server = new NamedPipeServerStream("LIFTOFF");

                await Task.Delay(4000);
                await Core.Log("Starting ROCKET...");
                rocket.Start();

                await Core.Log("Waiting for message from ROCKET...");
                server.WaitForConnection();

                StreamReader reader = new StreamReader(server);

                string response = reader.ReadLine();
                await Core.Log("Got response from ROCKET (" + response + ")");
                if (!bool.Parse(response.Split(",")[0]))
                {
                    string ex = response.Split(",")[1];
                    await Core.Log("Error launching game - " + ex);
                    MessageBox.Show("ERROR LAUNCHING GAME!" + Environment.NewLine +  ex);
                }
                else
                {
                    await Core.Log("Game launched!");
                }

                await rocket.WaitForExitAsync();
                await Core.Log("Game closed.");
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
                foreach (Game game in pgList)
                {
                    if (!Variables.GameList.Any(featGame => featGame.AppID == game.AppID))
                    {
                        Variables.GameList.Add(game);
                    }
                }
            }
        }

        public static async Task SetLocalGames()
        {
            List<Game> pgList = new List<Game>();

            foreach (Game g in Variables.GameList) pgList.Add(g);

            File.WriteAllText(Variables.ConfigDir + @"\mygames.json", JsonConvert.SerializeObject(pgList, Formatting.Indented));
        }

        public static async Task GetSteamGames()
        {
            await Core.Log("Getting Steam Games");
            Variables.SteamLibsFile = Functions.Core.getVariable("SteamLibsFile");
            await Core.Log("Steam Library File: " + Variables.SteamLibsFile);

            VProperty steamLibraries = VdfConvert.Deserialize(File.ReadAllText(Variables.SteamLibsFile));
            await Core.Log("Found " + steamLibraries.Value.Count() + " libraries...");

            foreach (VProperty steamLibrary in steamLibraries.Value)
            {
                int libraryID = 0;
                bool canRead = int.TryParse(steamLibrary.Key, out libraryID);
                if (canRead == true)
                {
                    await Core.Log("Found Steam Library (" + steamLibrary.Value["path"] + ")");
                    foreach (VProperty appid in steamLibrary.Value["apps"])
                    {
                        //Ignore official Steam Apps
                        if (appid.Key != "228980")
                        {
                            await Core.Log("Adding game " + appid.Key);
                            VProperty steamGameInfo = VdfConvert.Deserialize(File.ReadAllText(steamLibrary.Value["path"] + "\\steamapps\\appmanifest_" + appid.Key + ".acf"));

                            string name = steamGameInfo.Value["name"].Value<string>().ToLower();
                            if (!name.Contains("server") && !name.Contains("tool"))
                            {
                                Game pg = new Game();
                                pg.FileLocations = new List<string>();

                                pg.GameIcon = null;
                                pg.Title = steamGameInfo.Value["name"].Value<string>();
                                pg.AppID = uint.Parse(appid.Key.Replace("\"", ""));

                                string[] executables = Directory.GetFiles(steamLibrary.Value["path"] + "\\steamapps\\common\\" + steamGameInfo.Value["installdir"], "*.EXE", SearchOption.AllDirectories);
                                foreach (string exe in executables) pg.FileLocations.Add(exe);

                                pg.Filename = null;
                                pg.LinkURL = null;
                                pg.AdditionalArguments = "";

                                if (!Variables.GameList.Any(featGame => featGame.AppID == pg.AppID))
                                {
                                    Variables.GameList.Add(pg);
                                }
                            }
                            else await Core.Log(appid.Key + " is a server tool, ignoring.");
                        }
                    }
                }
                else await Core.Log("Couldn't read the library!", true);
            }
        }
    }
}
