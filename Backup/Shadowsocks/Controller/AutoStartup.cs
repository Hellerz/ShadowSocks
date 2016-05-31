namespace Shadowsocks.Controller
{
    using Microsoft.Win32;
    using System;
    using System.Windows.Forms;

    internal class AutoStartup
    {
        public static bool Check()
        {
            try
            {
                string executablePath = Application.ExecutablePath;
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                string[] valueNames = key.GetValueNames();
                key.Close();
                foreach (string str in valueNames)
                {
                    if (str.Equals("Shadowsocks"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                return false;
            }
        }

        public static bool Set(bool enabled)
        {
            try
            {
                string executablePath = Application.ExecutablePath;
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (enabled)
                {
                    key.SetValue("Shadowsocks", executablePath);
                }
                else
                {
                    key.DeleteValue("Shadowsocks");
                }
                key.Close();
                return true;
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                return false;
            }
        }
    }
}

