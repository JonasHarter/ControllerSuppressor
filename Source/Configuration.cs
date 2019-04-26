using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ControllerMapper.Source
{
    public class Configuration
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> processList { get; } = new List<string>();

        private static Configuration Instance;

        private Configuration() { }

        public static Configuration GetInstance()
        {
            if (Instance == null)
                Instance = Load();
            return Instance;
        }

        public bool Contains(string processName)
        {
            foreach (string configProcess in processList)
            {
                if (configProcess.ToLower().Equals(processName.ToLower()))
                    return true;
            }
            return false;
        }

        private void Save()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (StreamWriter writer = new StreamWriter(Program.configurationFilePath))
            {
                serializer.Serialize(writer.BaseStream, this);
            }
        }

        private static Configuration Load()
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Configuration));
                using (StreamReader reader = new StreamReader(Program.configurationFilePath))
                {
                    return deserializer.Deserialize(reader.BaseStream) as Configuration;
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Failed to load configuration file. Creating new one.");
                Configuration x = new Configuration();
                if (!File.Exists(Program.configurationFilePath))
                {
                    // Probably the common ones
                    x.processList.Add("XOutput.exe");
                    x.processList.Add("UCR.exe");
                    x.Save();
                }
                return x;
            }
        }
    }
}
