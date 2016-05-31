namespace Shadowsocks.Model
{
    using Shadowsocks.Controller;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    [Serializable]
    public class Server
    {
        public string method;
        public string password;
        public string remarks;
        public string server;
        public int server_port;

        public Server()
        {
            this.server = "";
            this.server_port = 0x20c4;
            this.method = "aes-256-cfb";
            this.password = "";
            this.remarks = "";
        }

        public Server(string ssURL) : this()
        {
            string s = Regex.Split(ssURL, "ss://", RegexOptions.IgnoreCase)[1].ToString();
            byte[] bytes = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    bytes = Convert.FromBase64String(s);
                }
                catch (FormatException)
                {
                    s = s + "=";
                }
            }
            if (bytes == null)
            {
                throw new FormatException();
            }
            try
            {
                string str2 = Encoding.UTF8.GetString(bytes);
                int length = str2.LastIndexOf('@');
                string str3 = str2.Substring(length + 1);
                int num3 = str3.LastIndexOf(':');
                this.server_port = int.Parse(str3.Substring(num3 + 1));
                this.server = str3.Substring(0, num3);
                string[] strArray2 = str2.Substring(0, length).Split(new char[] { ':' });
                this.method = strArray2[0];
                this.password = strArray2[1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new FormatException();
            }
        }

        public override bool Equals(object obj)
        {
            Server server = (Server) obj;
            return ((this.server == server.server) && (this.server_port == server.server_port));
        }

        public string FriendlyName()
        {
            if (string.IsNullOrEmpty(this.server))
            {
                return I18N.GetString("New server");
            }
            if (string.IsNullOrEmpty(this.remarks))
            {
                return (this.server + ":" + this.server_port);
            }
            return string.Concat(new object[] { this.remarks, " (", this.server, ":", this.server_port, ")" });
        }

        public override int GetHashCode()
        {
            return (this.server.GetHashCode() ^ this.server_port);
        }
    }
}

