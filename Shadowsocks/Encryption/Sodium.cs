namespace Shadowsocks.Encryption
{
    using Shadowsocks.Controller;
    using Shadowsocks.Properties;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class Sodium
    {
        private const string DLLNAME = "libsscrypto";

        static Sodium()
        {
            string fileName = Path.Combine(Path.GetTempPath(), "libsscrypto.dll");
            try
            {
                FileManager.UncompressFile(fileName, Resources.libsscrypto_dll);
                LoadLibrary(fileName);
            }
            catch (IOException)
            {
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            LoadLibrary(fileName);
        }

        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void crypto_stream_chacha20_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, ulong ic, byte[] k);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void crypto_stream_salsa20_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, ulong ic, byte[] k);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);
    }
}

