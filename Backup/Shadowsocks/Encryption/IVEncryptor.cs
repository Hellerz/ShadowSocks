namespace Shadowsocks.Encryption
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public abstract class IVEncryptor : EncryptorBase
    {
        protected int _cipher;
        protected int[] _cipherInfo;
        protected byte[] _decryptIV;
        protected int _decryptIVOffset;
        protected bool _decryptIVReceived;
        protected byte[] _encryptIV;
        protected int _encryptIVOffset;
        protected bool _encryptIVSent;
        protected byte[] _key;
        protected string _method;
        private static readonly Dictionary<string, byte[]> CachedKeys = new Dictionary<string, byte[]>();
        protected Dictionary<string, int[]> ciphers;
        protected int ivLen;
        protected int keyLen;
        protected static byte[] tempbuf = new byte[0x8000];

        public IVEncryptor(string method, string password) : base(method, password)
        {
            this.InitKey(method, password);
        }

        protected void bytesToKey(byte[] password, byte[] key)
        {
            byte[] array = new byte[password.Length + 0x10];
            int index = 0;
            byte[] buffer2 = null;
            while (index < key.Length)
            {
                MD5 md = MD5.Create();
                if (index == 0)
                {
                    buffer2 = md.ComputeHash(password);
                }
                else
                {
                    buffer2.CopyTo(array, 0);
                    password.CopyTo(array, buffer2.Length);
                    buffer2 = md.ComputeHash(array);
                }
                buffer2.CopyTo(key, index);
                index += buffer2.Length;
            }
        }

        protected abstract void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf);
        public override void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength)
        {
            if (!this._decryptIVReceived)
            {
                this._decryptIVReceived = true;
                this.initCipher(buf, false);
                outlength = length - this.ivLen;
                lock (tempbuf)
                {
                    Buffer.BlockCopy(buf, this.ivLen, tempbuf, 0, length - this.ivLen);
                    this.cipherUpdate(false, length - this.ivLen, tempbuf, outbuf);
                    return;
                }
            }
            outlength = length;
            this.cipherUpdate(false, length, buf, outbuf);
        }

        public override void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength)
        {
            if (!this._encryptIVSent)
            {
                this._encryptIVSent = true;
                randBytes(outbuf, this.ivLen);
                this.initCipher(outbuf, true);
                outlength = length + this.ivLen;
                lock (tempbuf)
                {
                    this.cipherUpdate(true, length, buf, tempbuf);
                    outlength = length + this.ivLen;
                    Buffer.BlockCopy(tempbuf, 0, outbuf, this.ivLen, length);
                    return;
                }
            }
            outlength = length;
            this.cipherUpdate(true, length, buf, outbuf);
        }

        protected abstract Dictionary<string, int[]> getCiphers();
        protected virtual void initCipher(byte[] iv, bool isCipher)
        {
            if (this.ivLen > 0)
            {
                if (isCipher)
                {
                    this._encryptIV = new byte[this.ivLen];
                    Array.Copy(iv, this._encryptIV, this.ivLen);
                }
                else
                {
                    this._decryptIV = new byte[this.ivLen];
                    Array.Copy(iv, this._decryptIV, this.ivLen);
                }
            }
        }

        protected void InitKey(string method, string password)
        {
            method = method.ToLower();
            this._method = method;
            string key = method + ":" + password;
            this.ciphers = this.getCiphers();
            this._cipherInfo = this.ciphers[this._method];
            this._cipher = this._cipherInfo[2];
            if (this._cipher == 0)
            {
                throw new Exception("method not found");
            }
            this.keyLen = this.ciphers[this._method][0];
            this.ivLen = this.ciphers[this._method][1];
            if (CachedKeys.ContainsKey(key))
            {
                this._key = CachedKeys[key];
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                this._key = new byte[0x20];
                this.bytesToKey(bytes, this._key);
                CachedKeys[key] = this._key;
            }
        }

        protected static void randBytes(byte[] buf, int length)
        {
            byte[] buffer = new byte[length];
            new Random().NextBytes(buffer);
            buffer.CopyTo(buf, 0);
        }
    }
}

