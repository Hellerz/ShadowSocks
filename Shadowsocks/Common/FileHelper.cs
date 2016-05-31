using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shadowsocks.Common
{
    public class FileHelper
    {
        private static string CONFIG_FILE_SUFFIX = "-config.json";

        public static string GetFilePath(string key)
        {
            return key + CONFIG_FILE_SUFFIX;
        }

        public static string GetConfig(string key, string defaultValue = "")
        {
            if (!File.Exists(GetFilePath(key)))
            {
                SaveConfig(key, defaultValue);
            }
            return File.ReadAllText(GetFilePath(key));
        }

        public static void SaveConfig(string key, string config)
        {
            try
            {
                using (var writer = new StreamWriter(File.Open(GetFilePath(key), FileMode.Create)))
                {
                    writer.Write(config);
                    writer.Flush();
                }
            }
            catch (IOException exception)
            {
                Console.Error.WriteLine(exception);
            }
        }
    }
}
