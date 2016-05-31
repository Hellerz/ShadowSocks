namespace Shadowsocks.Controller
{
    using Shadowsocks.Model;
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class UpdateChecker
    {
        public string LatestVersionNumber;
        public string LatestVersionURL;
        public EventHandler NewVersionFound;
        private const string UpdateURL = "https://api.github.com/repos/shadowsocks/shadowsocks-csharp/releases";
        public const string Version = "2.5.1";

        //public event EventHandler NewVersionFound
        //{
        //    add
        //    {
        //        EventHandler handler2;
        //        EventHandler newVersionFound = this.NewVersionFound;
        //        do
        //        {
        //            handler2 = newVersionFound;
        //            EventHandler handler3 = (EventHandler) Delegate.Combine(handler2, value);
        //            newVersionFound = Interlocked.CompareExchange<EventHandler>(ref this.NewVersionFound, handler3, handler2);
        //        }
        //        while (newVersionFound != handler2);
        //    }
        //    remove
        //    {
        //        EventHandler handler2;
        //        EventHandler newVersionFound = this.NewVersionFound;
        //        do
        //        {
        //            handler2 = newVersionFound;
        //            EventHandler handler3 = (EventHandler) Delegate.Remove(handler2, value);
        //            newVersionFound = Interlocked.CompareExchange<EventHandler>(ref this.NewVersionFound, handler3, handler2);
        //        }
        //        while (newVersionFound != handler2);
        //    }
        //}

        public void CheckUpdate(Configuration config)
        {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.3319.102 Safari/537.36");
            client.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("https://api.github.com/repos/shadowsocks/shadowsocks-csharp/releases"));
        }

        public static int CompareVersion(string l, string r)
        {
            string[] strArray = l.Split(new char[] { '.' });
            string[] strArray2 = r.Split(new char[] { '.' });
            for (int i = 0; i < Math.Max(strArray.Length, strArray2.Length); i++)
            {
                int num2 = (i < strArray.Length) ? int.Parse(strArray[i]) : 0;
                int num3 = (i < strArray2.Length) ? int.Parse(strArray2[i]) : 0;
                if (num2 != num3)
                {
                    return (num2 - num3);
                }
            }
            return 0;
        }

        private void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                JsonArray array = (JsonArray) SimpleJson.DeserializeObject(e.Result);
                List<string> versions = new List<string>();
                foreach (JsonObject obj2 in array)
                {
                    if (!((bool) obj2["prerelease"]))
                    {
                        foreach (JsonObject obj3 in (JsonArray) obj2["assets"])
                        {
                            string url = (string) obj3["browser_download_url"];
                            if (this.IsNewVersion(url))
                            {
                                versions.Add(url);
                            }
                        }
                        continue;
                    }
                }
                if (versions.Count != 0)
                {
                    this.SortVersions(versions);
                    this.LatestVersionURL = versions[versions.Count - 1];
                    this.LatestVersionNumber = ParseVersionFromURL(this.LatestVersionURL);
                    if (this.NewVersionFound != null)
                    {
                        this.NewVersionFound(this, new EventArgs());
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.Debug(exception.ToString());
            }
        }

        private bool IsNewVersion(string url)
        {
            if (url.IndexOf("prerelease") >= 0)
            {
                return false;
            }
            AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            System.Version version = Environment.Version;
            foreach (AssemblyName name in referencedAssemblies)
            {
                if (name.Name == "mscorlib")
                {
                    version = name.Version;
                }
            }
            if (version.Major >= 4)
            {
                if (url.IndexOf("dotnet4.0") < 0)
                {
                    return false;
                }
            }
            else if (url.IndexOf("dotnet4.0") >= 0)
            {
                return false;
            }
            string l = ParseVersionFromURL(url);
            if (l == null)
            {
                return false;
            }
            string r = "2.5.1";
            return (CompareVersion(l, r) > 0);
        }

        private static string ParseVersionFromURL(string url)
        {
            Match match = Regex.Match(url, @".*Shadowsocks-win.*?-([\d\.]+)\.\w+", RegexOptions.IgnoreCase);
            if (match.Success && (match.Groups.Count == 2))
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        private void SortVersions(List<string> versions)
        {
            versions.Sort(new VersionComparer());
        }

        public class VersionComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return UpdateChecker.CompareVersion(UpdateChecker.ParseVersionFromURL(x), UpdateChecker.ParseVersionFromURL(y));
            }
        }
    }
}

