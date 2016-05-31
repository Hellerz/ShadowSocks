namespace Shadowsocks.Controller
{
    using Shadowsocks.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class I18N
    {
        protected static Dictionary<string, string> Strings = new Dictionary<string, string>();

        static I18N()
        {
            if (CultureInfo.CurrentCulture.IetfLanguageTag.ToLowerInvariant().StartsWith("zh"))
            {
                foreach (string str in Regex.Split(Resources.cn, "\r\n|\r|\n"))
                {
                    if (!str.StartsWith("#"))
                    {
                        string[] strArray2 = Regex.Split(str, "=");
                        if (strArray2.Length == 2)
                        {
                            Strings[strArray2[0]] = strArray2[1];
                        }
                    }
                }
            }
        }

        public static string GetString(string key)
        {
            if (Strings.ContainsKey(key))
            {
                return Strings[key];
            }
            return key;
        }
    }
}

