namespace Shadowsocks.Encryption
{
    using System;
    using System.Runtime.InteropServices;

    public interface IEncryptor : IDisposable
    {
        void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength);
        void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength);
    }
}

