using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace LIFTOFF.Windows
{
    /// <summary>
    /// Interaction logic for Preloader.xaml
    /// </summary>
    public partial class Preloader : Window
    {
        #region Arguments
        private static List<Tuple<string, string>> CMDArgs = new List<Tuple<string, string>>();
        private void ArgHandler()
        {
            // Command line arguments
            //
            // CMD args are defines in pairs, this means that each arguments should have a key along with a value (ex. "-installed true -debug true -secretcode 101").
            //
            // Arguments:
            //
            // -debug (true|false)
            // This argument will display the application console along with avoiding installation.
            //
            // -installed (true|false)
            // This argument will indicate the install status of the application, if true the install was successful - if false it failed.

            string[] tmpArgs = string.Join(" ", App.StartupArgs).Split('-', StringSplitOptions.RemoveEmptyEntries);

            foreach (string arg in tmpArgs)
            {
                string[] argParts = arg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Tuple<string, string> newArg = new Tuple<string, string>(argParts[0], argParts[1]);
                CMDArgs.Add(newArg);

                //Argument handler
                switch (newArg)
                {
                    case Tuple<string, string>("debug", "true"):
                        AllocConsole();
                        Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Debug Console Enabled!");
                        Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: WARNING! Closing THIS console window manually can cause data loss!");
                        debug = true;
                        break;
                }
            }
        }
        #endregion

        #region Main
        public Preloader()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); }; //Draggy windows

            if (App.StartupArgs.Length != 0) ArgHandler();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Variables.AppVersion = "V. " + Assembly.GetEntryAssembly().GetName().Version.ToString();

            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: --- Application Details ---");
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Home Directory: " + Variables.HomeDir);
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Config Directory: " + Variables.ConfigDir);
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Application Version: " + Variables.AppVersion);
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Message of the day: " + await Variables.MOTD(true));
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: ---------------------------");

            PreloadManager();
        }


        public async Task PreloadManager()
        {

            await Status("Lifting off...");

            if (!CMDArgs.Contains(new Tuple<string, string>("installed", "true")))
            {
                await CheckInternetConnection();
                if (!internetConnected)
                {
                    await Status("Not Connected!");
                    MessageBox.Show("Please check your internet connection and try again :/");
                    await PreloadManager();
                }
                else await Status("Connected!");

                if (!debug) await CheckInstallation();
            }

            PreloadDone();
        }

        public async Task PreloadDone()
        {
            await Status("Done Preloading!", 650);

            Windows.Home home = new Windows.Home();
            home.Show();
            this.Close();
        }
        #endregion

        #region Checks
        private bool internetConnected = false;
        private async Task CheckInternetConnection()
        {
            ///
            ///Check client connection
            ///
            await Status("Checking internet...");

            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Running \"Pinger\" Task...");
            await Task.Run(() =>
            {
                Ping pinger = null;

                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Creating Ping...");
                try
                {
                    pinger = new Ping();

                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Sending Ping to 8.8.8.8");
                    PingReply reply = pinger.Send("8.8.8.8");

                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Got response!");
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Status: " + reply.Status);
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Reply-time: " + reply.RoundtripTime + "ms");

                    internetConnected = reply.Status == IPStatus.Success;
                }
                catch (PingException ex)
                {
                    Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: PING FAILED!");
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (pinger != null)
                    {
                        Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Disposing of \"Pinger\"");
                        pinger.Dispose();
                    }
                }
            });
        }

        public async Task CheckInstallation()
        {
            ///
            ///Check client installation
            ///
            await Status("Checking install....");
            if (!await Functions.Install.IsInstalled())
            {
                await Status("Not installed! Installing now...");
                if (!IsElevated) await RestartElevated();
                await Functions.Install.InstallLIFTOFF(this);
                await Status("Restarting...");

                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Starting new LIFTOFF service with args \"-installed true\"...");

                Process liftoffProcess = new Process();
                liftoffProcess.StartInfo.FileName = Variables.HomeDir + @"\LIFTOFF.exe";
                liftoffProcess.StartInfo.Arguments = "-installed true";
                liftoffProcess.Start();

                Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Launching CMD to remove the previous EXE...");

                ProcessStartInfo cmd = new ProcessStartInfo();
                cmd.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + System.Windows.Forms.Application.ExecutablePath;
                cmd.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.CreateNoWindow = true;
                cmd.FileName = "cmd.exe";
                Process.Start(cmd);

                Application.Current.Shutdown();
            }
            else await Status("installed!");
        }

        #endregion
        #region Mods
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public static bool debug = false;

        public bool IsElevated
        {
            get { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); }
        }

        internal async Task RestartElevated()
        {
            await Status("Restarting with install permissions...");
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Restarting with Admin permissions...");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                await Status("Restarting...");
                Process p = Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return;
            }
            Environment.Exit(0);
        }

        public async Task Status(string message, int wait = 20)
        {
            StatusTextBlock.Text = message.ToUpper();
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: " + message);
            await Task.Delay(wait);
        }
        #endregion
    }
}
