namespace Shadowsocks
{
    using Shadowsocks.Controller;
    using Shadowsocks.Util;
    using Shadowsocks.View;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Utils.ReleaseMemory();
            using (Mutex mutex = new Mutex(false, @"Global\71981632-A427-497F-AB91-241CD227EC1F"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (!mutex.WaitOne(0, false))
                {
                    Process[] processesByName = Process.GetProcessesByName("Shadowsocks");
                    if (processesByName.Length > 0)
                    {
                        Process process1 = processesByName[0];
                    }
                    MessageBox.Show("Shadowsocks is already running.\n\nFind Shadowsocks icon in your notify tray.");
                }
                else
                {
                    Directory.SetCurrentDirectory(Application.StartupPath);
                    Logging.OpenLogFile();
                    ShadowsocksController controller = new ShadowsocksController();
                    new MenuViewController(controller);
                    controller.Start();
                    Application.Run();
                }
            }
        }
    }
}

