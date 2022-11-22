using DiscordRPC;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Net;
using Newtonsoft.Json;

namespace LIFTOFF.Windows
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        #region Window Control

        /// <summary>
        /// The close button for this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            await Functions.Core.Log("Closing LIFTOFF...");
            if (!Maximized && !Fullscreen) Functions.Core.storeVariable("WindowSize", this.Width + "," + this.Height);
            Functions.Core.storeVariable("LastPageOpen", CurrentPage);
            if (Variables.CurrentGame.Title != "") Functions.Core.storeVariable("LastUsedGameAppID", Variables.CurrentGame.AppID.ToString());
            await Functions.Discord.discord.Deinitialize();

            if (Preloader.debug)
            {
                this.Visibility = Visibility.Collapsed;

                Functions.Core.Log("The application was closed successfully, you may not close this console.");
            }
            else Application.Current.Shutdown();


        }

        /// <summary>
        /// The Maximize/Restore button for this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool Maximized = false;

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Maximized)
            {
                Functions.Core.Log("Maximized off");
                MaxBtn.Content = "⧠";

                this.WindowState = WindowState.Normal;
                this.BorderThickness = new Thickness(2, 2, 2, 2);

                FullscreenBtn.IsEnabled = true;

                Maximized = false;
            }
            else
            {
                Functions.Core.Log("Maximized on");
                MaxBtn.Content = "⧉";

                this.MaxHeight = System.Windows.SystemParameters.MaximizedPrimaryScreenHeight;
                this.BorderThickness = new Thickness(5, 5, 5, 5);
                this.WindowState = WindowState.Maximized;

                FullscreenBtn.IsEnabled = false;

                Maximized = true;
            }
        }

        /// <summary>
        /// The Minimize button for this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Core.Log("Minimizing window...");
            this.WindowState = WindowState.Minimized;
        }


        private bool Fullscreen = false;
        /// <summary>
        /// The Fullscreen button for this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullscreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Fullscreen)
            {
                Functions.Core.Log("Fullscreen off");
                FullscreenBtn.Content = "⛗";

                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                this.BorderThickness = new Thickness(2,2,2,2);
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.WindowState = WindowState.Normal;

                Fullscreen = false;
            }
            else
            {
                Functions.Core.Log("Fullscreen on");
                FullscreenBtn.Content = "⛖";

                this.ResizeMode = ResizeMode.NoResize;
                this.MaxHeight = double.PositiveInfinity;
                this.BorderThickness = new Thickness(0,0,0,0);
                this.WindowState = WindowState.Maximized;

                Fullscreen = true;
            }
        }
        #endregion
        #region Pages Control
        private string CurrentPage = "";
        private void HidePages()
        {
            GamesPage.Visibility = Visibility.Hidden;
            ServersPage.Visibility = Visibility.Hidden;
            ModsPage.Visibility = Visibility.Hidden;
            HelpPage.Visibility = Visibility.Hidden;
            SettingsPage.Visibility = Visibility.Hidden;
            CreditsPage.Visibility = Visibility.Hidden;
            IntroPage.Visibility = Visibility.Hidden;
            PAGE_TEMPLATE.Visibility = Visibility.Hidden;
            ServersPage.Visibility = Visibility.Hidden;
            ServerPage.Visibility = Visibility.Hidden;
            EditGamePanel.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Overrides
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == Window.WindowStateProperty)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    MaxBtn.Content = "🗗";
                    Maximized = true;
                }
                else if (this.WindowState == WindowState.Normal)
                {
                    MaxBtn.Content = "◻";
                    Maximized = false;
                }
            }
        }
        #endregion
        #region Draggy Window and more
        private bool IsSecondClick = false;
        private DateTime clickedTime;
        private void DraggyWindow()
        {
            if (!Fullscreen)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
            }
        }
        private void MenuLogo_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HeaderLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Branding/Purple/Logo/Sizes/Logo1_Purple_150.png", UriKind.Absolute));
        }

        private void MenuLogo_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HeaderLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Branding/White/Logo/Sizes/Logo1_White_150.png", UriKind.Absolute));
        }

        private void HeaderLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenUrl("https://liftoff.publiczeus.com");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region Menu Buttons
        private async void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == "servers" && Variables.CurrentGame.Title != "")
            {
                Functions.Server currServer = (Functions.Server)ServerList.SelectedItem;
                if (currServer != null)
                {
                    if (!Variables.GameRunning)
                    {
                        Busy.Visibility = Visibility.Visible;
                        BusyTxtTitle.Text = "LAUNCHING " + Variables.CurrentGame.Title.ToUpper() + "...";
                        BusyTxtMessage.Text = "Joining server: \"" + currServer.Title + "\" (" + currServer.IPandPort() + ")";

                        if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
                        {
                            Functions.Discord.discord.client.SetPresence(new RichPresence()
                            {
                                Details = "Playing: " + currServer.Game.Title + "!",
                                State = currServer.Title + " | " + currServer.Info.map + "(" + currServer.Info.players + "/" + currServer.Info.max_players + ")",
                                Timestamps = Functions.Discord.startTime,
                                Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "Join Game", Url = "https://google.com" } },
                                Assets = new Assets()
                                {
                                    LargeImageKey = "logo_1000",
                                    LargeImageText = "LIFTOFF",
                                }
                            });
                        }
                        
                        Functions.Games.JoinGame(currServer);
                        ShowServersPage();
                    }
                    else MessageBox.Show("It looks like there is already a game running, try closing it and try again.");
                }
            }
            else ShowServersPage();
        }

        private async void RndServerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.CurrentGame.Title != "")
            {
                ServerList.ItemsSource = null;
                Busy.Visibility = Visibility.Visible;
                BusyTxtTitle.Text = "FINDING A RANDOM SERVER WITH PLAYERS...";
                BusyTxtMessage.Text = "Feeling lucky?";

                ObservableCollection<Functions.Server> servers = await Functions.Servers.GetServers(-1, this, "");

                IEnumerable<Functions.Server> serversWithPlayers = servers.Where(item => item.Info.players > 1);
                
                Random rnd = new Random();
                int i = rnd.Next(0, serversWithPlayers.Count());

                ServerList.Items.Clear();
                ShowServersPage(serversWithPlayers.ElementAt(i));

                Busy.Visibility = Visibility.Hidden;
            }
        }

        private async void GamesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesCombo.SelectedItem != null)
            {
                Variables.CurrentGame = (Functions.Game)GamesCombo.SelectedItem;

                EditGameBtn.IsEnabled = true;
                RemoveGameBtn.IsEnabled = true;
                RndServerBtn.IsEnabled = true;


                if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
                {
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = "Browsing their Games",
                        State = Variables.CurrentGame.Title,
                        Timestamps = Functions.Discord.startTime,
                        Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo_1000",
                            LargeImageText = "LIFTOFF",
                        }
                    });
                }
            }
            if (CurrentPage == "servers")
            {
                await DisplayServers();
            }
        }

        private async void GamesBtn_Click(object sender, RoutedEventArgs e)
        {
            Busy.Visibility = Visibility.Visible;
            BusyTxtTitle.Text = "GETTING YOUR GAMES...";
            if (CurrentPage != "games")
            {
                
                ShowGamesPage();
            }
        }

        private void ServersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != "servers") ShowServersPage();
        }

        private void ModsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != "mods") ShowModsPage();
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != "help") ShowHelpPage();
        }
        private void CreditsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != "credits") ShowCreditsPage();
        }
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != "settings") ShowSettingsPage();
        }
        #endregion

        #region Main
        public Home()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DraggyWindow(); }; //Draggy windows

            //Application Version
            VersionNumberTxtBox.Text = Variables.AppVersion;

            //Sets window size from last launch
            string winSize = Functions.Core.getVariable("WindowSize");
            if (winSize != "")
            {
                string[] winSizeWH = winSize.Split(',');
                this.Width = double.Parse(winSizeWH[0]);
                this.Height = double.Parse(winSizeWH[1]);
            }

            //Get Featured Games
            using (var client = new WebClient())
            {
                List<Functions.Game> featuredGames = new List<Functions.Game>();

                var content = client.DownloadData("https://liftoff.publiczeus.com/app/FeaturedGames.txt");
                using (var stream = new MemoryStream(content))
                {
                    StreamReader sr = new StreamReader(stream);
                    string json = sr.ReadToEnd();
                    if (json != "") featuredGames = JsonConvert.DeserializeObject<List<Functions.Game>>(json);
                }
                foreach (Functions.Game featuredGame in featuredGames)
                {
                    //Get Game file location via Steam

                    if (Functions.Games.IsGameInstalled(featuredGame.AppID).Result) Variables.GameList.Add(featuredGame);
                }

                client.Dispose();
            }

            //Gets Steam Games
            Functions.Games.GetSteamGames();

            //Sets tha Games combo box
            foreach (Functions.Game game in Variables.GameList)
            {
                DataGridRow row = new DataGridRow();
                row.Item = game;
                GamesCombo.Items.Add(row);
            }

            //Intro
            string introComplete = Functions.Core.getVariable("IntroComplete");
            if (introComplete == "" || !bool.Parse(introComplete))
            {
                //Show Intro
                ShowIntroPage();
            }
            else
            {
                //Set last used game
                string lastGame = Functions.Core.getVariable("LastUsedGameAppID");
                if (lastGame != "")
                {
                    uint appID = uint.Parse(lastGame);
                    Functions.Game game = Variables.GameList.FirstOrDefault(game => game.AppID == appID);
                    if (game != null)
                    {
                        Variables.CurrentGame = game;

                        GamesCombo.SelectedItem = game;
                    }
                }

                //Show last open Page
                string lastPage = Functions.Core.getVariable("LastPageOpen");
                if (lastPage != "" && Variables.CurrentGame.Title != "")
                {
                    switch (lastPage)
                    {
                        case "games":
                            ShowGamesPage();
                            break;
                        case "servers":
                            DisplayGames();
                            ShowServersPage();
                            break;
                        case "mods":
                            DisplayGames();
                            ShowModsPage();
                            break;
                        case "help":
                            DisplayGames();
                            ShowHelpPage();
                            break;
                        default:
                            ShowGamesPage();
                            break;
                    }
                }
                else ShowGamesPage();
            }
        }
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MOTDTxt.Text = await Variables.MOTD();
            doubleAnimation.From = -MOTDTxt.ActualWidth;
            doubleAnimation.To = canMain.ActualWidth;
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:12"));
            doubleAnimation.Completed += new EventHandler(MOTD_Completed);
            MOTDTxt.BeginAnimation(Canvas.LeftProperty, doubleAnimation);

            //Debug Section
        }
        private async void MOTD_Completed(object sender, EventArgs e)
        {
            MOTDTxt.Text = MOTDTxt.Text = await Variables.MOTD();
            doubleAnimation.From = -MOTDTxt.ActualWidth;
            doubleAnimation.To = canMain.ActualWidth;
            MOTDTxt.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
        }

        #endregion

        #region Games Page
        private async Task ShowGamesPage()
        {
            PlayBtn.IsEnabled = false;
            GamesBtn.Focus();
            HidePages();
            GamesPage.Visibility = Visibility.Visible;
            CurrentPage = "games";
            PageName.Text = CurrentPage.ToUpper();

            if (Variables.CurrentGame.Title != "")
            {
                EditGameBtn.IsEnabled = true;
                RemoveGameBtn.IsEnabled = true;
            }

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Browsing their Games",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }

            await DisplayGames();
        }

        private async void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Busy.Visibility = Visibility.Visible;
            BusyTxtTitle.Text = "ADDING GAME";
            BusyTxtMessage.Text = "Please select the game file... (\".exe\")";

            string initialDirectory()
            {
                string storedDir = Functions.Core.getVariable("LastSteamDir");
                if (storedDir != "")
                {
                    return storedDir;
                }
                else return @"C:\Program Files (x86)\Steam\steamapps\common";
            }


            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory(),
                Title = "Find Game Executable",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "exe",
                Filter = "EXE files|*.exe",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if ((bool)fileDialog.ShowDialog())
            {
                uint appID = 0;
                string plainName = "";

                if (fileDialog.FileName.ToLower().Contains("steam"))
                {
                    Functions.Core.storeVariable("LastSteamDir", Path.GetDirectoryName(fileDialog.FileName));
                    Process game = new Process();
                    game.StartInfo.FileName = fileDialog.FileName;
                    game.Start();
                    plainName = game.ProcessName;
                    game.Kill();

                    try
                    {
                        appID = uint.Parse(File.ReadAllText(fileDialog.FileName.Replace(fileDialog.SafeFileName, "steam_appid.txt")));

                        if (fileDialog.SafeFileName.Replace(".exe", "").Length > 2)
                        {
                            await Functions.Games.AddGame(new Functions.Game 
                            { 
                                Title = 
                                fileDialog.SafeFileName.Replace(".exe", "").ToUpper(), 
                                AppID = appID, 
                                //FileLocation = fileDialog.FileName, 
                                Plainname = plainName,
                                AdditionalArguments = ""
                            
                            });

                            Busy.Visibility = Visibility.Hidden;
                            await DisplayGames();
                        }
                        else
                        {
                            Busy.Visibility = Visibility.Hidden;
                            EditGame(fileDialog.SafeFileName.Replace(".exe", "").ToUpper(), appID, new List<string> { fileDialog.FileName }, plainName, null);
                        }
                        
                    }
                    catch
                    {
                        Busy.Visibility = Visibility.Hidden;
                        EditGame(fileDialog.SafeFileName.Replace(".exe", "").ToUpper(), appID, new List<string> { fileDialog.FileName }, plainName, null);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid Steam game!");
                    Busy.Visibility = Visibility.Hidden;
                }
            }
            else Busy.Visibility = Visibility.Hidden;
        }

        private Functions.Game GameBeingEdited = null;
        private void EditGameBtn_Click(object sender, RoutedEventArgs e)
        {
            EditGame(Variables.CurrentGame.Title, Variables.CurrentGame.AppID, Variables.CurrentGame.FileLocations, Variables.CurrentGame.BannerURL, Variables.CurrentGame.AdditionalArguments);
        }
        private void EditGame(string gameTitle, uint appID, List<string> fileLocations, string bannerURL, string arguments = "")
        {
            EditGameBG.Visibility = Visibility.Visible;

            EditGamePanel.Visibility = Visibility.Visible;

            GameNameTxt.Text = gameTitle;
            GameAppIDTxt.Text = appID.ToString();

            ExeCombo.ItemsSource = fileLocations;

            ArgumentsTxtBox.Text = arguments;
            BannerTxtBox.Text = bannerURL;


            GameBeingEdited = new Functions.Game()
            {
                Title = gameTitle,
                AppID = appID,
                //FileLocation = fileName,
                BannerURL = bannerURL,
            };
        }
        private async void SaveEditedServerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GameNameTxt.Text.Length > 2 && !Variables.GameList.Any(x => x.AppID == uint.Parse(GameAppIDTxt.Text)))
            {
                if (Variables.GameList.Any(x => x.AppID == GameBeingEdited.AppID)) Variables.GameList.Remove(Variables.CurrentGame);

                GameBeingEdited.Title = GameNameTxt.Text;
                GameBeingEdited.AppID = uint.Parse(GameAppIDTxt.Text);
                //GameBeingEdited.FileLocation = GameFileLocTxt.Text;
                GameBeingEdited.AdditionalArguments = ArgumentsTxtBox.Text;
                GameBeingEdited.BannerURL = BannerTxtBox.Text;

                await Functions.Games.AddGame(GameBeingEdited);
                Variables.CurrentGame = GameBeingEdited;
            }

            GameBeingEdited = null;

            EditGamePanel.Visibility = Visibility.Hidden;
            EditGameBG.Visibility = Visibility.Hidden;

            ShowGamesPage();
        }


        private async void RemoveGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Variables.GameList.Remove(Variables.CurrentGame);
            await Functions.Games.SetLocalGames();
            Variables.CurrentGame = new Functions.Game() { Title = "" };
            GamesCombo.SelectedIndex = -1;
            PlayBtn.IsEnabled = false;
            RndServerBtn.IsEnabled = false;

            EditGameBtn.IsEnabled = false;
            RemoveGameBtn.IsEnabled = false;

            ShowGamesPage();    
        }

        private void GameList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                Variables.CurrentGame = (Functions.Game)(GameList.SelectedItem as DataGridRow).Item;
                GamesCombo.SelectedItem = (Functions.Game)(GameList.SelectedItem as DataGridRow).Item;

                EditGameBtn.IsEnabled = true;
                RemoveGameBtn.IsEnabled = true;
                RndServerBtn.IsEnabled = true;

                if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
                {
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = "Browsing their Games",
                        State = Variables.CurrentGame.Title,
                        Timestamps = Functions.Discord.startTime,
                        Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo_1000",
                            LargeImageText = "LIFTOFF",
                        }
                    });
                }
            }
        }   

        private async Task DisplayGames()
        {
            GamesCombo.Items.Clear();
            GameList.Items.Clear();

            foreach (Functions.Game game in Variables.GameList.Where(game => game.Featured == true))
            {
                if (game.Featured)
                { 
                    DataGridRow row = new DataGridRow();

                    row.FontWeight = FontWeights.Bold;
                    row.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFF500"));
                    row.Item = game;

                    GameList.Items.Add(row);
                    GamesCombo.Items.Add(game);
                }
            }
            foreach (Functions.Game game in Variables.GameList.Where(game => game.Featured == false))
            {
                if (!game.Featured)
                {
                    DataGridRow row = new DataGridRow();
                    row.Item = game;

                    GameList.Items.Add(row);
                    GamesCombo.Items.Add(game);
                }
            }

            GamesCombo.SelectedIndex = Variables.GameList.FindIndex(item => item.AppID == Variables.CurrentGame.AppID);

            Busy.Visibility = Visibility.Hidden;
        }

        private void GameAppIDTxt_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion
        #region Servers Page
        private async void ShowServersPage(Functions.Server serverToDisplay = null)
        {
            ServersBtn.Focus();
            HidePages();
            ServersPage.Visibility = Visibility.Visible;
            CurrentPage = "servers";
            PageName.Text = CurrentPage.ToUpper();
            ServerList.ItemsSource = null;
            PlayBtn.IsEnabled = false;

            string savedServerCount = Functions.Core.getVariable("ServerCount");
            if (savedServerCount == "-1")
            {
                NumberToShow.Text = "";
            }
            else if (savedServerCount != "") NumberToShow.Text = savedServerCount;


            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Looking for Servers",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }

            if (Variables.CurrentGame.Title != "")
            {
                SelectAGameTxt.Visibility = Visibility.Hidden;
                RefreshBtn.IsEnabled = true;
                NumberToShow.IsEnabled = true;
                ServerList.IsEnabled = true;
                SearchBox.IsEnabled = true;
                if (serverToDisplay == null)
                {
                    if (ServerList.ItemsSource == null) await DisplayServers();
                }
                else
                {
                    ServerList.Items.Add(serverToDisplay);
                }
            }
            else
            {
                SelectAGameTxt.Visibility = Visibility.Visible;
                RefreshBtn.IsEnabled = false;
                NumberToShow.IsEnabled = false;
                ServerList.IsEnabled = false;
                SearchBox.IsEnabled = false;
            }
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NumberToShow.Text == "") Functions.Core.storeVariable("ServerCount", "-1");
            else Functions.Core.storeVariable("ServerCount", NumberToShow.Text);

            await DisplayServers();
        }

        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ServerList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ServerList.SelectedItem != null)
            {
                var serverItem = ServerList.SelectedItem;

                if ((serverItem as DataGridRow) != null)
                {
                    await ShowServerPage((ServerList.SelectedItem as DataGridRow).Item as Functions.Server);
                }
                else
                {
                    await ShowServerPage(ServerList.SelectedItem as Functions.Server);
                }
            }
        }

        private async Task ShowServerPage(Functions.Server Server)
        {
            ServerPage.Visibility = Visibility.Visible;
            string serverTitle = Server.Title.ToUpper() + " | Players: " + Server.Info.players + "/" + Server.Info.max_players;;

            long ping = long.Parse(Server.Ping);
            string ip = Server.IPandPort();
           
            if (ping < 90) PingTxt.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF23CE00"));
            else if (ping > 90 && ping < 300) PingTxt.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC3CE00"));
            else PingTxt.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF10CE00"));
            
            PingTxt.Text = ping.ToString() + "ms";
            IpTxt.Text = ip;
            
            ServerNameTxt.Text = serverTitle;

            string bannerURL = Server.FeaturedBanner;
            if (bannerURL == null)
            {
                bannerURL = "https://www.liftoff.publiczeus.com/app/ServerBanner.png";
            }

            ImageBrush brush = await Functions.Games.ImgBrushFromURL(bannerURL);
            ImageSource sourse = brush.ImageSource;
            ServerBanner.Source = sourse;
        }

        private void CloseServerBtn_Click(object sender, RoutedEventArgs e)
        {
            ServerPage.Visibility = Visibility.Hidden;
        }

        private void ServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ServerList.SelectedItem != null) PlayBtn.IsEnabled = true;
        }

        private void ServerList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //ServerList.Items.Refresh();
        }

        private async Task DisplayServers(bool Refresh = true)
        {
            PlayBtn.IsEnabled = false;
            if (ServerList.ItemsSource == null) ServerList.Items.Clear();
            ServerList.ItemsSource = null;

            Busy.Visibility = Visibility.Visible;
            if (NumberToShow.Text == "") BusyTxtTitle.Text = "LOOKING FOR SERVERS";
            else BusyTxtTitle.Text = "LOOKING FOR " + NumberToShow.Text + " SERVERS";

            BusyTxtMessage.Text = "Please wait...";

            if (SearchBox.Text != "") BusyTxtMessage.Text = "\r\nSearching for \"" + SearchBox.Text + "\"...";

            int serversToGet = -1;
            if (NumberToShow.Text != "") serversToGet = int.Parse(NumberToShow.Text);

            if (serversToGet > 1000 || serversToGet == -1) BusyTxtMessage.Text += "\r\n(Searching for more than 1000 servers could take a second)";

            if (Refresh) Variables.ServerList = await Functions.Servers.GetServers(serversToGet, this, SearchBox.Text);

            ServerList.ItemsSource = Variables.ServerList;

            if (ServerList.Items.Count == 0)
            {
                SelectAGameTxt.Visibility = Visibility.Visible;
                SelectAGameTxt.Text = "NO SERVERS FOUND :/";
                RefreshBtn.IsEnabled = false;
                NumberToShow.IsEnabled = false;
                ServerList.IsEnabled = false;
                SearchBox.IsEnabled = false;
            }
            else
            {
                SelectAGameTxt.Visibility = Visibility.Hidden;
                SelectAGameTxt.Text = "SELECT A GAME FIRST";
                RefreshBtn.IsEnabled = true;
                NumberToShow.IsEnabled = true;
                ServerList.IsEnabled = true;
                SearchBox.IsEnabled = true;
            }


            /*
            foreach (Functions.Server server in Variables.ServerList)
            {
                if (server.Featured)
                {
                    DataGridRow row = new DataGridRow();
                    server.Info.name = "❤️ " + server.Info.name;

                    row.FontWeight = FontWeights.Bold;
                    row.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFF500"));
                    row.Item = server;

                    ServerList.Items.Add(row);
                }
            }
            foreach (Functions.Server server in Variables.ServerList)
            {
                if (!server.Featured)
                {
                    ServerList.Items.Add(server);
                }
            }
            */

            Busy.Visibility = Visibility.Hidden;
        }

        private void ServerCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (SearchBox.Text != "")
            {
                ServerList.ItemsSource = null;
                ServerList.Items.Clear();
                foreach (Functions.Server server in Variables.ServerList.Where(stringToCheck => stringToCheck.Info.name.ToLower().Contains(SearchBox.Text.ToLower())).ToList<Functions.Server>())
                {
                    ServerList.Items.Add(server);
                }
            }
            else
            {
                await DisplayServers(false);
            }
        }

        #endregion
        #region Mods Page
        private void ShowModsPage()
        {
            PlayBtn.IsEnabled = false;
            ModsBtn.Focus();
            HidePages();
            ModsPage.Visibility = Visibility.Visible;
            CurrentPage = "mods";
            PageName.Text = CurrentPage.ToUpper();

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Checking Mods",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }
        }
        #endregion
        #region Help Page
        private void ShowHelpPage()
        {
            PlayBtn.IsEnabled = false;
            HelpBtn.Focus();
            HidePages();
            HelpPage.Visibility = Visibility.Visible;
            CurrentPage = "help";
            PageName.Text = CurrentPage.ToUpper();

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Looking for help",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }   
        }
        #endregion
        #region Credits Page
        private void ShowCreditsPage()
        {
            PlayBtn.IsEnabled = false;
            CreditsBtn.Focus();
            HidePages();
            CreditsPage.Visibility = Visibility.Visible;
            CurrentPage = "credits";
            PageName.Text = CurrentPage.ToUpper();

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Being Awesome!",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }
        }
        private void PublicZeusBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://publiczeus.com");
        }
        #endregion
        #region Settings Page
        private void ShowSettingsPage()
        {
            PlayBtn.IsEnabled = false;
            SettingsBtn.Focus();
            HidePages();
            SettingsPage.Visibility = Visibility.Visible;
            CurrentPage = "settings";
            PageName.Text = CurrentPage.ToUpper();

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Tweeking their Settings",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }
        }
        #endregion
        #region Intro Page
        private void ShowIntroPage()
        {
            PlayBtn.IsEnabled = false;
            GamesBtn.IsEnabled = false;
            ServersBtn.IsEnabled = false;
            ModsBtn.IsEnabled = false;
            HelpBtn.IsEnabled = false;
            FullscreenBtn.IsEnabled = false;
            SettingsBtn.IsEnabled = false;
            RndServerBtn.IsEnabled = false;
            GamesCombo.IsEnabled = false;
            CreditsBtn.IsEnabled = false;
            MinBtn.IsEnabled = false;
            MaxBtn.IsEnabled = false;

            HidePages();
            IntroPage.Visibility = Visibility.Visible;
            CurrentPage = "intro";
            PageName.Text = CurrentPage.ToUpper();

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Getting started...",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }
        }

        private async void IntroDone_Click(object sender, RoutedEventArgs e)
        {
            await IntroComplete();
            ShowGamesPage();
        }

        public async Task IntroComplete()
        {
            PlayBtn.IsEnabled = true;
            GamesBtn.IsEnabled = true;
            ServersBtn.IsEnabled = true;
            ModsBtn.IsEnabled = true;
            HelpBtn.IsEnabled = true;
            FullscreenBtn.IsEnabled = true;
            SettingsBtn.IsEnabled = true;
            RndServerBtn.IsEnabled = true;
            GamesCombo.IsEnabled = true;
            CreditsBtn.IsEnabled = true;
            MinBtn.IsEnabled = true;
            MaxBtn.IsEnabled = true;

            Functions.Core.storeVariable("IntroComplete", bool.TrueString);
        }

        #endregion

        #region PAGE_TEMPLATE
        private void ShowPAGE_NAMEPage()
        {
            PlayBtn.IsEnabled = false;
            HidePages();
            //PAGE_NAME.Visibility = Visibility.Visible;
            CurrentPage = "PAGE_NAME";
            PageName.Text = CurrentPage.ToUpper();

            if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
            {
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "DISCORD STATUS...",
                    State = Variables.CurrentGame.Title,
                    Timestamps = Functions.Discord.startTime,
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "GET LIFTOFF!", Url = "https://google.com" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo_1000",
                        LargeImageText = "LIFTOFF",
                    }
                });
            }
        }
        #endregion
    }
}
