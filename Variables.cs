using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace LIFTOFF
{
    class Variables
    {
        public static bool DiscordConnected = false;
        public static bool FirstRun = true;
        public static bool Updated = false;
        public static bool GameRunning = false;

        public static string AppVersion = "";

        public static ObservableCollection<Functions.Server> ServerList = null;

        public static Functions.Game CurrentGame = new Functions.Game() { Title = "" };
        public static List<Functions.Game> GameList = new List<Functions.Game>();

        public static string HomeDir = Environment.ExpandEnvironmentVariables("%ProgramFiles%") + @"\LIFTOFF";
        public static string ConfigDir = Environment.ExpandEnvironmentVariables("%AppData%") + @"\LIFTOFF";
    }
}
