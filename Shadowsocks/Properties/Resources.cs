namespace Shadowsocks.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), CompilerGenerated]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        internal static byte[] abp_js
        {
            get
            {
                return (byte[]) ResourceManager.GetObject("abp_js", resourceCulture);
            }
        }

        internal static string cn
        {
            get
            {
                return ResourceManager.GetString("cn", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static byte[] libsscrypto_dll
        {
            get
            {
                return (byte[]) ResourceManager.GetObject("libsscrypto_dll", resourceCulture);
            }
        }

        internal static string polipo_config
        {
            get
            {
                return ResourceManager.GetString("polipo_config", resourceCulture);
            }
        }

        internal static byte[] polipo_exe
        {
            get
            {
                return (byte[]) ResourceManager.GetObject("polipo_exe", resourceCulture);
            }
        }

        internal static byte[] proxy_pac_txt
        {
            get
            {
                return (byte[]) ResourceManager.GetObject("proxy_pac_txt", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Shadowsocks.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static Bitmap ss16
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("ss16", resourceCulture);
            }
        }

        internal static Bitmap ss20
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("ss20", resourceCulture);
            }
        }

        internal static Bitmap ss24
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("ss24", resourceCulture);
            }
        }

        internal static Bitmap ssw128
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("ssw128", resourceCulture);
            }
        }

        internal static string user_rule
        {
            get
            {
                return ResourceManager.GetString("user_rule", resourceCulture);
            }
        }
    }
}

