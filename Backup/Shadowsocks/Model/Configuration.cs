namespace Shadowsocks.Model
{
    using Shadowsocks.Controller;
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [Serializable]
    public class Configuration
    {
        private static string CONFIG_FILE = "gui-config.json";
        public List<Server> configs;
        public bool enabled;
        public bool global;
        public int index;
        public bool isDefault;
        public int localPort;
        public string pacUrl;
        public bool shareOverLan;
        public string strategy;
        public bool useOnlinePac;

        private static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception(I18N.GetString("assertion failure"));
            }
        }

        public static void CheckLocalPort(int port)
        {
            CheckPort(port);
            if (port == 0x1fbb)
            {
                throw new ArgumentException(I18N.GetString("Port can't be 8123"));
            }
        }

        private static void CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(I18N.GetString("Password can not be blank"));
            }
        }

        public static void CheckPort(int port)
        {
            if ((port <= 0) || (port > 0xffff))
            {
                throw new ArgumentException(I18N.GetString("Port out of range"));
            }
        }

        public static void CheckServer(Server server)
        {
            CheckPort(server.server_port);
            CheckPassword(server.password);
            CheckServer(server.server);
        }

        private static void CheckServer(string server)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentException(I18N.GetString("Server IP can not be blank"));
            }
        }

        public Server GetCurrentServer()
        {
            if ((this.index >= 0) && (this.index < this.configs.Count))
            {
                return this.configs[this.index];
            }
            return GetDefaultServer();
        }

        public static Server GetDefaultServer()
        {
            return new Server();
        }

        public static Configuration Load()
        {
            try
            {
                Configuration configuration = SimpleJson.SimpleJson.DeserializeObject<Configuration>(File.ReadAllText(CONFIG_FILE), new JsonSerializerStrategy());
                configuration.isDefault = false;
                if (configuration.localPort == 0)
                {
                    configuration.localPort = 0x438;
                }
                if ((configuration.index == -1) && (configuration.strategy == null))
                {
                    configuration.index = 0;
                }
                return configuration;
            }
            catch (Exception exception)
            {
                if (!(exception is FileNotFoundException))
                {
                    Console.WriteLine(exception);
                }
                Configuration configuration2 = new Configuration();
                configuration2.index = 0;
                configuration2.isDefault = true;
                configuration2.localPort = 0x438;
                List<Server> list = new List<Server>();
                list.Add(GetDefaultServer());
                configuration2.configs = list;
                return configuration2;
            }
        }

        public static void Save(Configuration config)
        {
            if (config.index >= config.configs.Count)
            {
                config.index = config.configs.Count - 1;
            }
            if (config.index < -1)
            {
                config.index = -1;
            }
            if ((config.index == -1) && (config.strategy == null))
            {
                config.index = 0;
            }
            config.isDefault = false;
            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open(CONFIG_FILE, FileMode.Create)))
                {
                    string str = SimpleJson.SimpleJson.SerializeObject(config);
                    writer.Write(str);
                    writer.Flush();
                }
            }
            catch (IOException exception)
            {
                Console.Error.WriteLine(exception);
            }
        }

        private class JsonSerializerStrategy : PocoJsonSerializerStrategy
        {
            public override object DeserializeObject(object value, Type type)
            {
                if ((type == typeof(int)) && (value.GetType() == typeof(string)))
                {
                    return int.Parse(value.ToString());
                }
                return base.DeserializeObject(value, type);
            }
        }
    }
}

