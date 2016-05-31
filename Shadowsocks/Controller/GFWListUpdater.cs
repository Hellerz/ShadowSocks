namespace Shadowsocks.Controller
{
    using Shadowsocks.Model;
    using Shadowsocks.Properties;
    using Shadowsocks.Util;
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class GFWListUpdater
    {
        public ErrorEventHandler Error;
        private const string GFWLIST_URL = "https://autoproxy-gfwlist.googlecode.com/svn/trunk/gfwlist.txt";
        private static string PAC_FILE = PACServer.PAC_FILE;
        public EventHandler<ResultEventArgs> UpdateCompleted;
        private static string USER_RULE_FILE = PACServer.USER_RULE_FILE;

        //public event ErrorEventHandler Error
        //{
        //    add
        //    {
        //        ErrorEventHandler handler2;
        //        ErrorEventHandler error = this.Error;
        //        do
        //        {
        //            handler2 = error;
        //            ErrorEventHandler handler3 = (ErrorEventHandler) Delegate.Combine(handler2, value);
        //            error = Interlocked.CompareExchange<ErrorEventHandler>(ref this.Error, handler3, handler2);
        //        }
        //        while (error != handler2);
        //    }
        //    remove
        //    {
        //        ErrorEventHandler handler2;
        //        ErrorEventHandler error = this.Error;
        //        do
        //        {
        //            handler2 = error;
        //            ErrorEventHandler handler3 = (ErrorEventHandler) Delegate.Remove(handler2, value);
        //            error = Interlocked.CompareExchange<ErrorEventHandler>(ref this.Error, handler3, handler2);
        //        }
        //        while (error != handler2);
        //    }
        //}

        //public event EventHandler<ResultEventArgs> UpdateCompleted
        //{
        //    add
        //    {
        //        EventHandler<ResultEventArgs> handler2;
        //        EventHandler<ResultEventArgs> updateCompleted = this.UpdateCompleted;
        //        do
        //        {
        //            handler2 = updateCompleted;
        //            EventHandler<ResultEventArgs> handler3 = (EventHandler<ResultEventArgs>) Delegate.Combine(handler2, value);
        //            updateCompleted = Interlocked.CompareExchange<EventHandler<ResultEventArgs>>(ref this.UpdateCompleted, handler3, handler2);
        //        }
        //        while (updateCompleted != handler2);
        //    }
        //    remove
        //    {
        //        EventHandler<ResultEventArgs> handler2;
        //        EventHandler<ResultEventArgs> updateCompleted = this.UpdateCompleted;
        //        do
        //        {
        //            handler2 = updateCompleted;
        //            EventHandler<ResultEventArgs> handler3 = (EventHandler<ResultEventArgs>) Delegate.Remove(handler2, value);
        //            updateCompleted = Interlocked.CompareExchange<EventHandler<ResultEventArgs>>(ref this.UpdateCompleted, handler3, handler2);
        //        }
        //        while (updateCompleted != handler2);
        //    }
        //}

        private void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                List<string> json = this.ParseResult(e.Result);
                if (File.Exists(USER_RULE_FILE))
                {
                    foreach (string str2 in File.ReadAllText(USER_RULE_FILE, Encoding.UTF8).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!str2.StartsWith("!") && !str2.StartsWith("["))
                        {
                            json.Add(str2);
                        }
                    }
                }
                string contents = Utils.UnGzip(Resources.abp_js).Replace("__RULES__", SimpleJson.SerializeObject(json));
                if (File.Exists(PAC_FILE) && (File.ReadAllText(PAC_FILE, Encoding.UTF8) == contents))
                {
                    this.UpdateCompleted(this, new ResultEventArgs(false));
                }
                else
                {
                    File.WriteAllText(PAC_FILE, contents, Encoding.UTF8);
                    if (this.UpdateCompleted != null)
                    {
                        this.UpdateCompleted(this, new ResultEventArgs(true));
                    }
                }
            }
            catch (Exception exception)
            {
                if (this.Error != null)
                {
                    this.Error(this, new ErrorEventArgs(exception));
                }
            }
        }

        public List<string> ParseResult(string response)
        {
            byte[] bytes = Convert.FromBase64String(response);
            string[] strArray = Encoding.ASCII.GetString(bytes).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>(strArray.Length);
            foreach (string str2 in strArray)
            {
                if (!str2.StartsWith("!") && !str2.StartsWith("["))
                {
                    list.Add(str2);
                }
            }
            return list;
        }

        public void UpdatePACFromGFWList(Configuration config)
        {
            WebClient client = new WebClient();
            client.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("https://autoproxy-gfwlist.googlecode.com/svn/trunk/gfwlist.txt"));
        }

        public class ResultEventArgs : EventArgs
        {
            public bool Success;

            public ResultEventArgs(bool success)
            {
                this.Success = success;
            }
        }
    }
}

