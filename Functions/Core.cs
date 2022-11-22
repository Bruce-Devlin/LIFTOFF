using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace LIFTOFF.Functions
{
    class Core
    {
        static string configFile = Variables.ConfigDir + "\\user.config";
        public static void storeVariable(string name, string data)
        {
            Log("Writing variable \"" + name + "\" with value \"" + data + "\" to the config file...");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            if (settings[name] == null) settings.Add(name, data);
            else settings[name].Value = data;
            configuration.Save(ConfigurationSaveMode.Modified);
        }

        public static string getVariable(string variable)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;

            string value;
            if (settings[variable] == null) value = "";
            else value = settings[variable].Value;

            return value;
        }

        public static async Task Log(string mssgToLog, bool error = false)
        {
            if (error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Console.Write("[" + timeStamp + "] LIFTOFF: ");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(mssgToLog);
        }
    }
}
