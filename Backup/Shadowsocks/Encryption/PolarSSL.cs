namespace Shadowsocks.Encryption
{
    using Shadowsocks.Controller;
    using Shadowsocks.Properties;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class PolarSSL
    {
        public const int AES_CTX_SIZE = 280;
        public const int AES_DECRYPT = 0;
        public const int AES_ENCRYPT = 1;
        public const int ARC4_CTX_SIZE = 0x108;
        private const string DLLNAME = "libsscrypto";

        static PolarSSL()
        {
            string fileName = Path.GetTempPath() + "/libsscrypto.dll";
            try
            {
                FileManager.UncompressFile(fileName, Resources.libsscrypto_dll);
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
        public static extern int aes_crypt_cfb128(IntPtr ctx, int mode, int length, ref int iv_off, byte[] iv, byte[] input, byte[] output);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void aes_free(IntPtr ctx);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void aes_init(IntPtr ctx);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern int aes_setkey_enc(IntPtr ctx, byte[] key, int keysize);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern int arc4_crypt(IntPtr ctx, int length, byte[] input, byte[] output);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void arc4_free(IntPtr ctx);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void arc4_init(IntPtr ctx);
        [DllImport("libsscrypto", CallingConvention=CallingConvention.Cdecl)]
        public static extern void arc4_setup(IntPtr ctx, byte[] key, int keysize);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);
    }
}

