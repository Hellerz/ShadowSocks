namespace Shadowsocks.Controller
{
    using Microsoft.Win32;
    using Shadowsocks.Model;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class SystemProxy
    {
        private static bool _refreshReturn;
        private static bool _settingsReturn;
        public const int INTERNET_OPTION_REFRESH = 0x25;
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 0x27;

        private static void CopyProxySettingFromLan()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections", true);
            object obj2 = key.GetValue("DefaultConnectionSettings");
            try
            {
                foreach (string str in key.GetValueNames())
                {
                    if ((!str.Equals("DefaultConnectionSettings") && !str.Equals("LAN Connection")) && !str.Equals("SavedLegacySettings"))
                    {
                        key.SetValue(str, obj2);
                    }
                }
                NotifyIE();
            }
            catch (IOException exception)
            {
                Logging.LogUsefulException(exception);
            }
        }

        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public static void NotifyIE()
        {
            _settingsReturn = InternetSetOption(IntPtr.Zero, 0x27, IntPtr.Zero, 0);
            _refreshReturn = InternetSetOption(IntPtr.Zero, 0x25, IntPtr.Zero, 0);
        }

        public static void Update(Configuration config, bool forceDisable)
        {
            bool global = config.global;
            bool enabled = config.enabled;
            if (forceDisable)
            {
                enabled = false;
            }
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
                if (enabled)
                {
                    if (global)
                    {
                        key.SetValue("ProxyEnable", 1);
                        key.SetValue("ProxyServer", "127.0.0.1:" + config.localPort.ToString());
                        key.SetValue("AutoConfigURL", "");
                    }
                    else
                    {
                        string pacUrl;
                        if (config.useOnlinePac && !string.IsNullOrEmpty(config.pacUrl))
                        {
                            pacUrl = config.pacUrl;
                        }
                        else
                        {
                            pacUrl = "http://127.0.0.1:" + config.localPort.ToString() + "/pac?t=" + GetTimestamp(DateTime.Now);
                        }
                        key.SetValue("ProxyEnable", 0);
                        key.SetValue("ProxyServer", "");
                        key.SetValue("AutoConfigURL", pacUrl);
                    }
                }
                else
                {
                    key.SetValue("ProxyEnable", 0);
                    key.SetValue("ProxyServer", "");
                    key.SetValue("AutoConfigURL", "");
                }
                NotifyIE();
                CopyProxySettingFromLan();
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                MessageBox.Show(I18N.GetString("Failed to update registry"));
            }
        }
    }
}

