using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using FluentScheduler;
using Jurassic;
using Newtonsoft.Json;
using Shadowsocks.Common;
using Shadowsocks.Model;

namespace Shadowsocks.View
{
    public class AuthorityMannager
    {
        private const string Url = "url";
        private const string AnalyzeJs = "analyzeJS";

        public static bool CheckConnection()
        {
            var isConnection = true;
            WebHelper.ExecuteHttp(Common.Common.CheckConnectionUrl, (
                request, response) =>
            {
                isConnection = true;
            }, (
                request, exception) =>
            {
                isConnection = false;
            });
            return isConnection;
        }

        public static void Update()
        {
            var url = FileHelper.GetConfig(Url, Common.Common.ServerDataUrl);
            WebHelper.ExecuteHttp(url, (request, response) =>
            {
                var html = WebHelper.GetHtmlString(response);
                var server = AnalyzeAuthority(html);
                var config = Configuration.Load();
                if (!config.configs.CanBeCount())
                {
                    config.configs=new List<Server>{server};
                }
                config.configs[0] = server;
                Program.Controller.SaveConfig(config);
            });
        }

        public static Server AnalyzeAuthority(string htmlString)
        {
            var engine = new ScriptEngine();
            engine.SetGlobalValue("input", htmlString);
            engine.ExecuteFile(FileHelper.GetFilePath(AnalyzeJs));
            var output = engine.GetGlobalValue<string>("output");
            if (!string.IsNullOrWhiteSpace(output) && output != "undefined" && output != "null")
            {
                return JsonConvert.DeserializeObject<Server>(output);
            }
            return null;
        }
    }
}
