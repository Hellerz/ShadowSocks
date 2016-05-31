using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shadowsocks.Controller;

namespace Shadowsocks.Common
{
    public static class Common
    {
        public const string ServerDataUrl = "http://www.feixunwangluo.com/page/testss.html";

        public const string CheckConnectionUrl = "https://www.google.com/";

        public const int DefaultAutoIntervel = 15;
        public static bool CanBeCount<T>(this ICollection<T> collection, int minCount = 1, int maxCount = int.MaxValue)
        {
            return collection != null && collection.Count >= minCount && collection.Count <= maxCount;
        }
    }
}
