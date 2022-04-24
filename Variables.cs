using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;

namespace LIFTOFF
{
    class Variables
    {
        public static bool DiscordConnected = false;
        public static bool FirstRun = true;
        public static bool Updated = false;
        public static bool GameRunning = false;

        public static string AppVersion = "";

        private static int currentMessage = 1;
        public static async Task<string> MOTD(bool displayFull = false)
        {
            string motd = "";
            using (var client = new WebClient())
            {
                var content = client.DownloadData("https://liftoff.publiczeus.com/app/motd.txt");
                using (var stream = new MemoryStream(content))
                {
                    StreamReader sr = new StreamReader(stream);
                    motd = sr.ReadToEnd().ToUpper();
                }
            }

            if (!displayFull)
            {
                string[] messages = motd.Split(";", StringSplitOptions.TrimEntries);
                string messageToSend = messages[currentMessage-1];

                if (currentMessage == messages.Length) currentMessage = 1;
                else currentMessage++;
                return messageToSend;
            }
            else return motd;
            
        }

        public static ObservableCollection<Functions.Server> ServerList = null;

        public static Functions.Game CurrentGame = new Functions.Game() { Title = "" };
        public static List<Functions.Game> GameList = new List<Functions.Game>();

        public static string HomeDir = Environment.ExpandEnvironmentVariables("%ProgramFiles%") + @"\LIFTOFF";
        public static string ConfigDir = Environment.ExpandEnvironmentVariables("%AppData%") + @"\LIFTOFF";
    }
}
