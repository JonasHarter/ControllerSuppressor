using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ControllerMapper.Source
{
    public class Configuration
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> HidList { get; } = new List<string>();

        private static Configuration Instance;

        private Configuration() { }

        public static Configuration GetInstance()
        {
            if (Instance == null)
                Instance = Load();
            return Instance;
        }

        public bool Contains(string hid)
        {
            foreach (string listHid in HidList)
            {
                if (listHid.Contains(hid))
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
            // TODO don'T write file as admin
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f85c9b29-b5ff-4ddb-b8fd-57fb71f5cb08/c-file-write-using-another-account-also-changed-file-privilege-how-to-avoid-it?forum=csharpgeneral
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
                    x.HidList.Add("HID\\VID_0F0D&PID_00DC&IG_00");
                    //x.HidList.Add("HID\\VID_045E&PID_02FFC&IG_00");
                    x.Save();
                }
                return x;
            }
        }
    }
}
