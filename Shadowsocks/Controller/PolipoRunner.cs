namespace Shadowsocks.Controller
{
    using Shadowsocks.Model;
    using Shadowsocks.Properties;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;

    internal class PolipoRunner
    {
        private Process _process;
        private int _runningPort;
        private static string temppath = Path.GetTempPath();

        static PolipoRunner()
        {
            try
            {
                FileManager.UncompressFile(temppath + "/ss_polipo.exe", Resources.polipo_exe);
            }
            catch (IOException exception)
            {
                Logging.LogUsefulException(exception);
            }
        }

        private int GetFreePort()
        {
            int num = 0x1fbb;
            try
            {
                IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                List<int> list = new List<int>();
                foreach (IPEndPoint point in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
                {
                    list.Add(point.Port);
                }
                for (int i = num; i <= 0xffff; i++)
                {
                    if (!list.Contains(i))
                    {
                        return i;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                return num;
            }
            throw new Exception("No free port found.");
        }

        public void Start(Configuration configuration)
        {
            configuration.GetCurrentServer();
            if (this._process == null)
            {
                foreach (Process process in Process.GetProcessesByName("ss_polipo"))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                }
                string s = Resources.polipo_config;
                this._runningPort = this.GetFreePort();
                s = s.Replace("__SOCKS_PORT__", configuration.localPort.ToString()).Replace("__POLIPO_BIND_PORT__", this._runningPort.ToString()).Replace("__POLIPO_BIND_IP__", configuration.shareOverLan ? "0.0.0.0" : "127.0.0.1");
                FileManager.ByteArrayToFile(temppath + "/polipo.conf", Encoding.UTF8.GetBytes(s));
                this._process = new Process();
                this._process.StartInfo.FileName = temppath + "/ss_polipo.exe";
                this._process.StartInfo.Arguments = "-c \"" + temppath + "/polipo.conf\"";
                this._process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                this._process.StartInfo.UseShellExecute = true;
                this._process.StartInfo.CreateNoWindow = true;
                this._process.Start();
            }
        }

        public void Stop()
        {
            if (this._process != null)
            {
                try
                {
                    this._process.Kill();
                    this._process.WaitForExit();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
                this._process = null;
            }
        }

        public int RunningPort
        {
            get
            {
                return this._runningPort;
            }
        }
    }
}

