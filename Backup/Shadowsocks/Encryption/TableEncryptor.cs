namespace Shadowsocks.Encryption
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class TableEncryptor : EncryptorBase
    {
        private readonly byte[] _decryptTable;
        private readonly byte[] _encryptTable;

        public TableEncryptor(string method, string password) : base(method, password)
        {
            this._encryptTable = new byte[0x100];
            this._decryptTable = new byte[0x100];
            ulong a = BitConverter.ToUInt64(base.GetPasswordHash(), 0);
            for (int i = 0; i < 0x100; i++)
            {
                this._encryptTable[i] = (byte) i;
            }
            for (int j = 1; j < 0x400; j++)
            {
                this._encryptTable = this.MergeSort(this._encryptTable, a, j);
            }
            for (int k = 0; k < 0x100; k++)
            {
                this._decryptTable[this._encryptTable[k]] = (byte) k;
            }
        }

        private static long Compare(byte x, byte y, ulong a, int i)
        {
            return (long) ((a % ((long) (x + i))) - (a % ((long) (y + i))));
        }

        public override void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength)
        {
            for (int i = 0; i < length; i++)
            {
                outbuf[i] = this._decryptTable[buf[i]];
            }
            outlength = length;
        }

        public override void Dispose()
        {
        }

        public override void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength)
        {
            for (int i = 0; i < length; i++)
            {
                outbuf[i] = this._encryptTable[buf[i]];
            }
            outlength = length;
        }

        private byte[] MergeSort(byte[] array, ulong a, int j)
        {
            if (array.Length == 1)
            {
                return array;
            }
            int num = array.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = array[i];
            }
            byte[] buffer2 = new byte[array.Length - num];
            for (int k = 0; k < (array.Length - num); k++)
            {
                buffer2[k] = array[k + num];
            }
            buffer = this.MergeSort(buffer, a, j);
            buffer2 = this.MergeSort(buffer2, a, j);
            int index = 0;
            int num5 = 0;
            byte[] buffer3 = new byte[array.Length];
            for (int m = 0; m < array.Length; m++)
            {
                if ((num5 == buffer2.Length) || ((index < buffer.Length) && (Compare(buffer[index], buffer2[num5], a, j) <= 0L)))
                {
                    buffer3[m] = buffer[index];
                    index++;
                }
                else if ((index == buffer.Length) || ((num5 < buffer2.Length) && (Compare(buffer2[num5], buffer[index], a, j) <= 0L)))
                {
                    buffer3[m] = buffer2[num5];
                    num5++;
                }
            }
            return buffer3;
        }

        public static List<string> SupportedCiphers()
        {
            return new List<string>(new string[] { "table" });
        }
    }
}

