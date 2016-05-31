namespace Shadowsocks.Encryption
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public class PolarSSLEncryptor : IVEncryptor, IDisposable
    {
        private static Dictionary<string, int[]> _ciphers;
        private IntPtr _decryptCtx;
        private bool _disposed;
        private IntPtr _encryptCtx;
        private const int CIPHER_AES = 1;
        private const int CIPHER_RC4 = 2;

        static PolarSSLEncryptor()
        {
            Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>();
            dictionary.Add("aes-128-cfb", new int[] { 0x10, 0x10, 1, 280 });
            dictionary.Add("aes-192-cfb", new int[] { 0x18, 0x10, 1, 280 });
            dictionary.Add("aes-256-cfb", new int[] { 0x20, 0x10, 1, 280 });
            dictionary.Add("rc4", new int[] { 0x10, 0, 2, 0x108 });
            dictionary.Add("rc4-md5", new int[] { 0x10, 0x10, 2, 0x108 });
            _ciphers = dictionary;
        }

        public PolarSSLEncryptor(string method, string password) : base(method, password)
        {
            this._encryptCtx = IntPtr.Zero;
            this._decryptCtx = IntPtr.Zero;
            base.InitKey(method, password);
        }

        protected override void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf)
        {
            byte[] buffer;
            int num;
            IntPtr ptr;
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (isCipher)
            {
                buffer = base._encryptIV;
                num = base._encryptIVOffset;
                ptr = this._encryptCtx;
            }
            else
            {
                buffer = base._decryptIV;
                num = base._decryptIVOffset;
                ptr = this._decryptCtx;
            }
            switch (base._cipher)
            {
                case 1:
                    PolarSSL.aes_crypt_cfb128(ptr, isCipher ? 1 : 0, length, ref num, buffer, buf, outbuf);
                    if (!isCipher)
                    {
                        base._decryptIVOffset = num;
                        return;
                    }
                    base._encryptIVOffset = num;
                    return;

                case 2:
                    PolarSSL.arc4_crypt(ptr, length, buf, outbuf);
                    return;
            }
        }

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (this._disposed)
                {
                    return;
                }
                this._disposed = true;
            }
            if (disposing)
            {
                if (this._encryptCtx != IntPtr.Zero)
                {
                    switch (base._cipher)
                    {
                        case 1:
                            PolarSSL.aes_free(this._encryptCtx);
                            break;

                        case 2:
                            PolarSSL.arc4_free(this._encryptCtx);
                            break;
                    }
                    Marshal.FreeHGlobal(this._encryptCtx);
                    this._encryptCtx = IntPtr.Zero;
                }
                if (this._decryptCtx != IntPtr.Zero)
                {
                    switch (base._cipher)
                    {
                        case 1:
                            PolarSSL.aes_free(this._decryptCtx);
                            break;

                        case 2:
                            PolarSSL.arc4_free(this._decryptCtx);
                            break;
                    }
                    Marshal.FreeHGlobal(this._decryptCtx);
                    this._decryptCtx = IntPtr.Zero;
                }
            }
        }

        ~PolarSSLEncryptor()
        {
            this.Dispose(false);
        }

        protected override Dictionary<string, int[]> getCiphers()
        {
            return _ciphers;
        }

        protected override void initCipher(byte[] iv, bool isCipher)
        {
            byte[] buffer;
            base.initCipher(iv, isCipher);
            IntPtr ctx = Marshal.AllocHGlobal(base._cipherInfo[3]);
            if (isCipher)
            {
                this._encryptCtx = ctx;
            }
            else
            {
                this._decryptCtx = ctx;
            }
            if (base._method == "rc4-md5")
            {
                byte[] destinationArray = new byte[base.keyLen + base.ivLen];
                buffer = new byte[base.keyLen];
                Array.Copy(base._key, 0, destinationArray, 0, base.keyLen);
                Array.Copy(iv, 0, destinationArray, base.keyLen, base.ivLen);
                buffer = MD5.Create().ComputeHash(destinationArray);
            }
            else
            {
                buffer = base._key;
            }
            if (base._cipher == 1)
            {
                PolarSSL.aes_init(ctx);
                PolarSSL.aes_setkey_enc(ctx, buffer, base.keyLen * 8);
            }
            else if (base._cipher == 2)
            {
                PolarSSL.arc4_init(ctx);
                PolarSSL.arc4_setup(ctx, buffer, base.keyLen);
            }
        }

        public static List<string> SupportedCiphers()
        {
            return new List<string>(_ciphers.Keys);
        }
    }
}

