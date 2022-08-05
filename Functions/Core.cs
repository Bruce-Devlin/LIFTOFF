using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace LIFTOFF.Functions
{
    class Core
    {
        static string configFile = Variables.ConfigDir + "\\user.config";
        public static void storeVariable(string name, string data)
        {
            Core.Log("Writing variable \"" + name + "\" with value \"" + data + "\" to the config file...");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            if (settings[name] == null) settings.Add(name, data);
            else settings[name].Value = data;
            configuration.Save(ConfigurationSaveMode.Modified);
            Core.Log("Done writing.");
        }

        public static string getVariable(string variable)
        {
            Log("Reading value for variable \"" + variable + "\"");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            Core.Log("Done reading.");

            if (settings[variable] == null) return "";
            else return settings[variable].Value;
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
                Console.Write("[" + DateTime.Now + "] LIFTOFF: ");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(mssgToLog);
        }
    }
}
