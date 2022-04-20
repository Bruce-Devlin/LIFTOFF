using System;
using System.Configuration;
using System.IO;

namespace LIFTOFF.Functions
{
    class Config
    {
        static string configFile = Variables.ConfigDir + "\\user.config";
        public static void storeVariable(string name, string data)
        {
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Writing variable \"" + name + "\" with value \"" + data + "\" to the config file...");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            if (settings[name] == null) settings.Add(name, data);
            else settings[name].Value = data;
            configuration.Save(ConfigurationSaveMode.Modified);
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Done writing.");
        }

        public static string getVariable(string variable)
        {
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Reading value for variable \"" + variable + "\"");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            Console.WriteLine("[" + DateTime.Now + "] LIFTOFF: Done reading.");

            if (settings[variable] == null) return "";
            else return settings[variable].Value;
        }
    }
}
