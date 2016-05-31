namespace Shadowsocks.Controller
{
    using Shadowsocks.Model;
    using Shadowsocks.Properties;
    using Shadowsocks.Util;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    internal class PACServer : Listener.Service
    {
        private Configuration _config;
        public static string PAC_FILE = "pac.txt";
        private EventHandler PACFileChanged;
        public static string USER_RULE_FILE = "user-rule.txt";
        private FileSystemWatcher watcher;

        public event EventHandler PACFileChanged
        {
            add
            {
                EventHandler handler2;
                EventHandler pACFileChanged = this.PACFileChanged;
                do
                {
                    handler2 = pACFileChanged;
                    EventHandler handler3 = (EventHandler) Delegate.Combine(handler2, value);
                    pACFileChanged = Interlocked.CompareExchange<EventHandler>(ref this.PACFileChanged, handler3, handler2);
                }
                while (pACFileChanged != handler2);
            }
            remove
            {
                EventHandler handler2;
                EventHandler pACFileChanged = this.PACFileChanged;
                do
                {
                    handler2 = pACFileChanged;
                    EventHandler handler3 = (EventHandler) Delegate.Remove(handler2, value);
                    pACFileChanged = Interlocked.CompareExchange<EventHandler>(ref this.PACFileChanged, handler3, handler2);
                }
                while (pACFileChanged != handler2);
            }
        }

        public PACServer()
        {
            this.WatchPacFile();
        }

        private string GetPACAddress(byte[] requestBuf, int length, IPEndPoint localEndPoint, bool useSocks)
        {
            return string.Concat(new object[] { useSocks ? "SOCKS5 " : "PROXY ", localEndPoint.Address, ":", this._config.localPort, ";" });
        }

        private string GetPACContent()
        {
            if (File.Exists(PAC_FILE))
            {
                return File.ReadAllText(PAC_FILE, Encoding.UTF8);
            }
            return Utils.UnGzip(Resources.proxy_pac_txt);
        }

        public bool Handle(byte[] firstPacket, int length, Socket socket, object state)
        {
            if (socket.ProtocolType != ProtocolType.Tcp)
            {
                return false;
            }
            try
            {
                string[] strArray = Encoding.UTF8.GetString(firstPacket, 0, length).Split(new char[] { '\r', '\n' });
                bool flag = false;
                bool flag2 = false;
                bool useSocks = false;
                foreach (string str2 in strArray)
                {
                    string[] strArray2 = str2.Split(new char[] { ':' }, 2);
                    if (strArray2.Length == 2)
                    {
                        if (strArray2[0] == "Host")
                        {
                            if (strArray2[1].Trim() == ((IPEndPoint) socket.LocalEndPoint).ToString())
                            {
                                flag = true;
                            }
                        }
                        else if (strArray2[0] == "User-Agent")
                        {
                        }
                    }
                    else if ((strArray2.Length == 1) && (str2.IndexOf("pac") >= 0))
                    {
                        flag2 = true;
                    }
                }
                if (flag && flag2)
                {
                    this.SendResponse(firstPacket, length, socket, useSocks);
                    return true;
                }
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket asyncState = (Socket) ar.AsyncState;
            try
            {
                asyncState.Shutdown(SocketShutdown.Send);
            }
            catch
            {
            }
        }

        public void SendResponse(byte[] firstPacket, int length, Socket socket, bool useSocks)
        {
            try
            {
                string pACContent = this.GetPACContent();
                IPEndPoint localEndPoint = (IPEndPoint) socket.LocalEndPoint;
                string newValue = this.GetPACAddress(firstPacket, length, localEndPoint, useSocks);
                pACContent = pACContent.Replace("__PROXY__", newValue);
                string s = string.Format("HTTP/1.1 200 OK\r\nServer: Shadowsocks\r\nContent-Type: application/x-ns-proxy-autoconfig\r\nContent-Length: {0}\r\nConnection: Close\r\n\r\n", Encoding.UTF8.GetBytes(pACContent).Length) + pACContent;
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), socket);
                Utils.ReleaseMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                socket.Close();
            }
        }

        public string TouchPACFile()
        {
            if (!File.Exists(PAC_FILE))
            {
                FileManager.UncompressFile(PAC_FILE, Resources.proxy_pac_txt);
            }
            return PAC_FILE;
        }

        internal string TouchUserRuleFile()
        {
            if (!File.Exists(USER_RULE_FILE))
            {
                File.WriteAllText(USER_RULE_FILE, Resources.user_rule);
            }
            return USER_RULE_FILE;
        }

        public void UpdateConfiguration(Configuration config)
        {
            this._config = config;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (this.PACFileChanged != null)
            {
                this.PACFileChanged(this, new EventArgs());
            }
        }

        private void WatchPacFile()
        {
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
            this.watcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
            this.watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
            this.watcher.Filter = PAC_FILE;
            this.watcher.Changed += new FileSystemEventHandler(this.Watcher_Changed);
            this.watcher.Created += new FileSystemEventHandler(this.Watcher_Changed);
            this.watcher.Deleted += new FileSystemEventHandler(this.Watcher_Changed);
            this.watcher.Renamed += new RenamedEventHandler(this.Watcher_Changed);
            this.watcher.EnableRaisingEvents = true;
        }
    }
}

