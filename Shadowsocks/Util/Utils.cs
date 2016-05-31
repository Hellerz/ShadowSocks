namespace Shadowsocks.Util
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using System.Text;

    public class Utils
    {
        public static void ReleaseMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            //SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, long minimumWorkingSetSize, long maximumWorkingSetSize);
        public static string UnGzip(byte[] buf)
        {
            byte[] buffer = new byte[0x400];
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream stream2 = new GZipStream(new MemoryStream(buf), CompressionMode.Decompress, false))
                {
                    int num;
                    while ((num = stream2.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, num);
                    }
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}

