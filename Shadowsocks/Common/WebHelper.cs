using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Shadowsocks.Common
{
    public static class WebHelper
    {
        private static readonly Encoding WebEncoding = Encoding.UTF8;
        public static void ExecuteHttp(string url, Action<HttpWebRequest, HttpWebResponse> finishHandler, Action<HttpWebRequest, Exception> errorHandler = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3";
            request.Timeout = 5000;
            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                if (finishHandler != null)
                {
                    finishHandler(request, response);
                }
            }
            catch (WebException e)
            {
                if (errorHandler != null)
                {
                    errorHandler(request,e);
                }
            }
            finally
            {
                request.Abort();
            }
        }

        public static string GetHtmlString(HttpWebResponse response)
        {
            var retString = string.Empty;
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                    {
                        using (var decompressStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        {
                            using (var streamReader = new StreamReader(decompressStream, WebEncoding))
                            {
                                retString = streamReader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        using (var streamReader = new StreamReader(responseStream, WebEncoding))
                        {
                            retString = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return retString;
        }
    }
}
