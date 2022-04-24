using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LIFTOFF.Functions
{
    class Install
    {
        public static async Task<bool> IsInstalled()
        {
            if (System.Windows.Forms.Application.ExecutablePath.StartsWith(Variables.HomeDir))
                return true;
            else return false;
        }

        public static async Task InstallLIFTOFF(Windows.Preloader preloader)
        {
            try
            {
                string pathToExe = Variables.HomeDir + "\\LIFTOFF.exe";

                if (!Directory.Exists(Variables.HomeDir)) Directory.CreateDirectory(Variables.HomeDir);

                if (System.IO.File.Exists(pathToExe)) System.IO.File.Delete(pathToExe);
                System.IO.File.Copy(System.Windows.Forms.Application.ExecutablePath, pathToExe);

                await DownloadFile(Variables.HomeDir + "\\ROCKET.exe", "https://liftoff.publiczeus.com/app/ROCKET.exe", preloader);
                await DownloadFile(Variables.HomeDir + "\\steam_api64.dll", "https://liftoff.publiczeus.com/app/steam_api64.dll", preloader);

            
                string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "LIFTOFF");

                if (!Directory.Exists(appStartMenuPath))
                    Directory.CreateDirectory(appStartMenuPath);

                string shortcutLocation = Path.Combine(appStartMenuPath, "LIFTOFF" + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "The LIFTOFF Launcher!";
                shortcut.TargetPath = pathToExe;
                shortcut.Save();

                string debugShortcutLocation = Path.Combine(Variables.HomeDir, "LIFTOFF (debug)" + ".lnk");
                IWshShortcut debugShortcut = (IWshShortcut)shell.CreateShortcut(debugShortcutLocation);

                debugShortcut.Description = "Launch LIFTOFF in debug mode!";
                debugShortcut.Arguments = "-debug true";
                debugShortcut.TargetPath = pathToExe;
                debugShortcut.Save();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static async Task DownloadFile(string location, string url, Windows.Preloader preloader)
        {
            bool completed = false;
            using (WebClient webClient = new WebClient())
            {
               
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    preloader.Status("DOWNLOADING... (" + e.ProgressPercentage.ToString() + "%)");
                });

                webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(delegate (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                {
                    preloader.Status("DOWNLOAD COMPLETE!");
                    completed = true;
                });
                webClient.DownloadFileAsync(new Uri(url), location);

                
            }
            do { await Task.Delay(1000); } while (!completed);
        }
    }
}
