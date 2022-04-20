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
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Closing LIFTOFF...");
            if (!Maximized && !Fullscreen) Functions.Config.storeVariable("WindowSize", this.Width + "," + this.Height);
            Functions.Config.storeVariable("LastPageOpen", CurrentPage);
            if (Variables.CurrentGame.Title != "") Functions.Config.storeVariable("LastUsedGameAppID", Variables.CurrentGame.AppID.ToString());
            await Functions.Discord.discord.Deinitialize();

            if (Preloader.debug)
            {
                this.Visibility = Visibility.Collapsed;

                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: The application was closed successfully, you may not close this console.");
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
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Maximized off");
                MaxBtn.Content = "⧠";

                this.WindowState = WindowState.Normal;
                this.BorderThickness = new Thickness(2, 2, 2, 2);

                FullscreenBtn.IsEnabled = true;

                Maximized = false;
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Maximized on");
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
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Minimizing window...");
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
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Fullscreen off");
                FullscreenBtn.Content = "⛗";

                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                this.BorderThickness = new Thickness(2,2,2,2);
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.WindowState = WindowState.Normal;

                Fullscreen = false;
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Fullscreen on");
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
                if (IsSecondClick)
                {
                    IsSecondClick = false;
                    if ((clickedTime - DateTime.Now).Milliseconds > -300)
                    {
                        Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Double click detected");
                        if (Maximized)
                        {
                            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Maximized off");
                            MaxBtn.Content = "◻";

                            this.WindowState = WindowState.Normal;
                            this.BorderThickness = new Thickness(2, 2, 2, 2);

                            FullscreenBtn.IsEnabled = true;

                            Maximized = false;
                        }
                        else
                        {
                            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Maximized on");
                            MaxBtn.Content = "🗗";

                            this.MaxHeight = System.Windows.SystemParameters.MaximizedPrimaryScreenHeight;
                            this.BorderThickness = new Thickness(5, 5, 5, 5);
                            this.WindowState = WindowState.Maximized;

                            FullscreenBtn.IsEnabled = false;

                            Maximized = true;
                        }
                    }
                }
                else
                {
                    IsSecondClick = true;
                    clickedTime = DateTime.Now;
                }
                DragMove();
            }
        }
        private void MenuHeader_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HeaderLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Branding/Purple/Logo/Sizes/Logo1_Purple_150.png", UriKind.Absolute));
        }

        private void MenuHeader_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HeaderLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Branding/White/Logo/Sizes/Logo1_White_150.png", UriKind.Absolute));
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
                        BusyTxtMessage.Text = "Joining server: \"" + currServer.Info.name + "\" (" + currServer.IpandPort() + ")";

                        if (Functions.Discord.DiscordAlive() && !Variables.GameRunning)
                        {
                            Functions.Discord.discord.client.SetPresence(new RichPresence()
                            {
                                Details = "Playing: " + currServer.Game.Title + "!",
                                State = currServer.Info.name + " | " + currServer.Info.map + "(" + currServer.Info.players + "/" + currServer.Info.max_players + ")",
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

        private void GamesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                ShowServersPage();
            }
        }

        private void GamesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != "games") ShowGamesPage();
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
            string winSize = Functions.Config.getVariable("WindowSize");
            if (winSize != "")
            {
                string[] winSizeWH = winSize.Split(',');
                this.Width = double.Parse(winSizeWH[0]);
                this.Height = double.Parse(winSizeWH[1]);
            }

            //Gets the game list
            Functions.Games.GetLocalGames();
            GamesCombo.ItemsSource = Variables.GameList;

            //Set last used game
            string lastGame = Functions.Config.getVariable("LastUsedGameAppID");
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
            string lastPage = Functions.Config.getVariable("LastPageOpen");
            if (lastPage != "" && Variables.CurrentGame.Title != "")
            {
                switch (lastPage)
                {
                    case "games":
                        ShowGamesPage();
                        break;
                    case "servers":
                        ShowServersPage();
                        break;
                    case "mods":
                        ShowModsPage();
                        break;
                    case "help":
                        ShowHelpPage();
                        break;
                    default:
                        ShowGamesPage();
                        break;
                }
            }
            else ShowGamesPage();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Games
        private async void ShowGamesPage()
        {
            PlayBtn.IsEnabled = false;
            GamesBtn.Focus();
            HidePages();
            GamesPage.Visibility = Visibility.Visible;
            CurrentPage = "games";

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
                string storedDir = Functions.Config.getVariable("LastSteamDir");
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
                    Functions.Config.storeVariable("LastSteamDir", Path.GetDirectoryName(fileDialog.FileName));
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
                                FileLocation = fileDialog.FileName, 
                                Plainname = plainName,
                                AdditionalArguments = ""
                            
                            });

                            Busy.Visibility = Visibility.Hidden;
                            await DisplayGames();
                        }
                        else
                        {
                            Busy.Visibility = Visibility.Hidden;
                            EditGame(fileDialog.SafeFileName.Replace(".exe", "").ToUpper(), appID, fileDialog.FileName, plainName, null);
                        }
                        
                    }
                    catch
                    {
                        Busy.Visibility = Visibility.Hidden;
                        EditGame(fileDialog.SafeFileName.Replace(".exe", "").ToUpper(), appID, fileDialog.FileName, plainName, null);
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
            EditGame(Variables.CurrentGame.Title, Variables.CurrentGame.AppID, Variables.CurrentGame.FileLocation, Variables.CurrentGame.Plainname, Variables.CurrentGame.BannerURL, Variables.CurrentGame.AdditionalArguments);
        }
        private void EditGame(string gameTitle, uint appID, string fileName, string plainName, string bannerURL, string arguments = "")
        {
            EditGameBG.Visibility = Visibility.Visible;

            EditGamePanel.Visibility = Visibility.Visible;

            GameNameTxt.Text = gameTitle;
            GameAppIDTxt.Text = appID.ToString();
            GameFileLocTxt.Text = fileName;
            ArgumentsTxtBox.Text = arguments;
            BannerTxtBox.Text = bannerURL;


            GameBeingEdited = new Functions.Game()
            {
                Title = gameTitle,
                AppID = appID,
                FileLocation = fileName,
                BannerURL = bannerURL,
                Plainname = plainName,
                ServerAppID = 0
            };
        }
        private async void SaveEditedServerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GameNameTxt.Text.Length > 2)
            {
                if (Variables.GameList.Any(x => x.AppID == GameBeingEdited.AppID)) Variables.GameList.Remove(Variables.CurrentGame);

                GameBeingEdited.Title = GameNameTxt.Text;
                GameBeingEdited.AppID = uint.Parse(GameAppIDTxt.Text);
                GameBeingEdited.FileLocation = GameFileLocTxt.Text;
                GameBeingEdited.AdditionalArguments = ArgumentsTxtBox.Text;
                GameBeingEdited.BannerURL = BannerTxtBox.Text;

                await Functions.Games.AddGame(GameBeingEdited);
                Variables.CurrentGame = GameBeingEdited;
                GameBeingEdited = null;

                EditGamePanel.Visibility = Visibility.Hidden;
                EditGameBG.Visibility = Visibility.Hidden;

                ShowGamesPage();
            }
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
                Variables.CurrentGame = (Functions.Game)GameList.SelectedItem;
                GamesCombo.SelectedItem = (Functions.Game)GameList.SelectedItem;

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
            if (GameList.ItemsSource != null) GameList.ItemsSource = null;
            if (GamesCombo.ItemsSource != null) GamesCombo.ItemsSource = null;

            Busy.Visibility = Visibility.Visible;
            BusyTxtTitle.Text = "CHECKING GAMES";
            BusyTxtMessage.Text = "Please wait...";

            GameList.ItemsSource = Variables.GameList;

            GamesCombo.ItemsSource = Variables.GameList;

            GamesCombo.SelectedIndex = Variables.GameList.FindIndex(item => item.AppID == Variables.CurrentGame.AppID);

            Busy.Visibility = Visibility.Hidden;
        }

        private void GameAppIDTxt_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion
        #region Servers
        private async void ShowServersPage(Functions.Server serverToDisplay = null)
        {
            ServersBtn.Focus();
            HidePages();
            ServersPage.Visibility = Visibility.Visible;
            CurrentPage = "servers";
            ServerList.Items.Clear();
            PlayBtn.IsEnabled = false;

            string savedServerCount = Functions.Config.getVariable("ServerCount");
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
                if (serverToDisplay == null) await DisplayServers();
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
            if (NumberToShow.Text == "") Functions.Config.storeVariable("ServerCount", "-1");
            else Functions.Config.storeVariable("ServerCount", NumberToShow.Text);

            await DisplayServers();
        }

        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ServerList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ServerList.SelectedItem != null)
            {
                MessageBox.Show("Info for server: " + (ServerList.SelectedItem as Functions.Server).Info.name);
            }
        }

        private void ServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ServerList.SelectedItem != null) PlayBtn.IsEnabled = true;
        }

        private async Task DisplayServers()
        {
            PlayBtn.IsEnabled = false;
            ServerList.Items.Clear();

            Busy.Visibility = Visibility.Visible;
            if (NumberToShow.Text == "") BusyTxtTitle.Text = "LOOKING FOR ALL SERVERS";
            else BusyTxtTitle.Text = "LOOKING FOR " + NumberToShow.Text + " SERVERS";

            BusyTxtMessage.Text = "Please wait...";

            if (SearchBox.Text != "") BusyTxtMessage.Text = "\r\nSearching for \"" + SearchBox.Text + "\"...";

            int serversToGet = -1;
            if (NumberToShow.Text != "") serversToGet = int.Parse(NumberToShow.Text);

            if (serversToGet > 1000 || serversToGet == -1) BusyTxtMessage.Text += "\r\n(Searching for more than 1000 servers could take a second)";

            Variables.ServerList = await Functions.Servers.GetServers(serversToGet, this, SearchBox.Text);

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

            Busy.Visibility = Visibility.Hidden;
        }

        private void ServerCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (SearchBox.Text != "")
            {
                ServerList.ItemsSource = Variables.ServerList.Where(stringToCheck => stringToCheck.Info.name.ToLower().Contains(SearchBox.Text.ToLower())).ToList<Functions.Server>();
            }
            else
            {
                ServerList.ItemsSource = Variables.ServerList;
            }
        }

        #endregion
        #region Mods
        private void ShowModsPage()
        {
            PlayBtn.IsEnabled = false;
            ModsBtn.Focus();
            HidePages();
            ModsPage.Visibility = Visibility.Visible;
            CurrentPage = "mods";

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
        #region Help
        private void ShowHelpPage()
        {
            PlayBtn.IsEnabled = false;
            HelpBtn.Focus();
            HidePages();
            HelpPage.Visibility = Visibility.Visible;
            CurrentPage = "help";

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
        #region Credits
        private void ShowCreditsPage()
        {
            PlayBtn.IsEnabled = false;
            CreditsBtn.Focus();
            HidePages();
            CreditsPage.Visibility = Visibility.Visible;
            CurrentPage = "credits";

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
        #region Settings
        private void ShowSettingsPage()
        {
            PlayBtn.IsEnabled = false;
            SettingsBtn.Focus();
            HidePages();
            SettingsPage.Visibility = Visibility.Visible;
            CurrentPage = "settings";

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
    }
}
