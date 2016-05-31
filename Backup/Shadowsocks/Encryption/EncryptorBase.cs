namespace Shadowsocks.Encryption
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public abstract class EncryptorBase : IEncryptor, IDisposable
    {
        public const int MAX_INPUT_SIZE = 0x8000;
        protected string Method;
        protected string Password;

        protected EncryptorBase(string method, string password)
        {
            this.Method = method;
            this.Password = password;
        }

        public abstract void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength);
        public abstract void Dispose();
        public abstract void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength);
        protected byte[] GetPasswordHash()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(this.Password);
            return MD5.Create().ComputeHash(bytes);
        }
    }
}

