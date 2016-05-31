namespace Shadowsocks.Encryption
{
    using System;
    using System.Collections.Generic;

    public class SodiumEncryptor : IVEncryptor, IDisposable
    {
        private static Dictionary<string, int[]> _ciphers;
        protected byte[] _decryptBuf;
        protected int _decryptBytesRemaining;
        protected ulong _decryptIC;
        protected byte[] _encryptBuf;
        protected int _encryptBytesRemaining;
        protected ulong _encryptIC;
        private const int CIPHER_CHACHA20 = 2;
        private const int CIPHER_SALSA20 = 1;
        private const int SODIUM_BLOCK_SIZE = 0x40;

        static SodiumEncryptor()
        {
            Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>();
            dictionary.Add("salsa20", new int[] { 0x20, 8, 1, 280 });
            dictionary.Add("chacha20", new int[] { 0x20, 8, 2, 280 });
            _ciphers = dictionary;
        }

        public SodiumEncryptor(string method, string password) : base(method, password)
        {
            base.InitKey(method, password);
            this._encryptBuf = new byte[0x8040];
            this._decryptBuf = new byte[0x8040];
        }

        protected override void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf)
        {
            int num;
            ulong num2;
            byte[] buffer;
            byte[] buffer2;
            if (isCipher)
            {
                num = this._encryptBytesRemaining;
                num2 = this._encryptIC;
                buffer = this._encryptBuf;
                buffer2 = base._encryptIV;
            }
            else
            {
                num = this._decryptBytesRemaining;
                num2 = this._decryptIC;
                buffer = this._decryptBuf;
                buffer2 = base._decryptIV;
            }
            int dstOffset = num;
            Buffer.BlockCopy(buf, 0, buffer, dstOffset, length);
            switch (base._cipher)
            {
                case 1:
                    Sodium.crypto_stream_salsa20_xor_ic(buffer, buffer, (ulong) (dstOffset + length), buffer2, num2, base._key);
                    break;

                case 2:
                    Sodium.crypto_stream_chacha20_xor_ic(buffer, buffer, (ulong) (dstOffset + length), buffer2, num2, base._key);
                    break;
            }
            Buffer.BlockCopy(buffer, dstOffset, outbuf, 0, length);
            dstOffset += length;
            num2 += (ulong) (((long) dstOffset) / 0x40L);
            num = dstOffset % 0x40;
            if (isCipher)
            {
                this._encryptBytesRemaining = num;
                this._encryptIC = num2;
            }
            else
            {
                this._decryptBytesRemaining = num;
                this._decryptIC = num2;
            }
        }

        public override void Dispose()
        {
        }

        protected override Dictionary<string, int[]> getCiphers()
        {
            return _ciphers;
        }

        public static List<string> SupportedCiphers()
        {
            return new List<string>(_ciphers.Keys);
        }
    }
}

