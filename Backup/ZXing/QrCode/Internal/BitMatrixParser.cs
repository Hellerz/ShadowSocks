namespace ZXing.QrCode.Internal
{
    using System;
    using ZXing.Common;

    internal sealed class BitMatrixParser
    {
        private readonly BitMatrix bitMatrix;
        private bool mirrored;
        private FormatInformation parsedFormatInfo;
        private Version parsedVersion;

        private BitMatrixParser(BitMatrix bitMatrix)
        {
            this.bitMatrix = bitMatrix;
        }

        private int copyBit(int i, int j, int versionBits)
        {
            if (!(this.mirrored ? this.bitMatrix[j, i] : this.bitMatrix[i, j]))
            {
                return (versionBits << 1);
            }
            return ((versionBits << 1) | 1);
        }

        internal static BitMatrixParser createBitMatrixParser(BitMatrix bitMatrix)
        {
            int height = bitMatrix.Height;
            if ((height >= 0x15) && ((height & 3) == 1))
            {
                return new BitMatrixParser(bitMatrix);
            }
            return null;
        }

        internal void mirror()
        {
            for (int i = 0; i < this.bitMatrix.Width; i++)
            {
                for (int j = i + 1; j < this.bitMatrix.Height; j++)
                {
                    if (this.bitMatrix[i, j] != this.bitMatrix[j, i])
                    {
                        this.bitMatrix.flip(j, i);
                        this.bitMatrix.flip(i, j);
                    }
                }
            }
        }

        internal byte[] readCodewords()
        {
            FormatInformation information = this.readFormatInformation();
            if (information == null)
            {
                return null;
            }
            Version version = this.readVersion();
            if (version == null)
            {
                return null;
            }
            DataMask mask = DataMask.forReference(information.DataMask);
            int height = this.bitMatrix.Height;
            mask.unmaskBitMatrix(this.bitMatrix, height);
            BitMatrix matrix = version.buildFunctionPattern();
            bool flag = true;
            byte[] buffer = new byte[version.TotalCodewords];
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            for (int i = height - 1; i > 0; i -= 2)
            {
                if (i == 6)
                {
                    i--;
                }
                for (int j = 0; j < height; j++)
                {
                    int num7 = flag ? ((height - 1) - j) : j;
                    for (int k = 0; k < 2; k++)
                    {
                        if (!matrix[i - k, num7])
                        {
                            num4++;
                            num3 = num3 << 1;
                            if (this.bitMatrix[i - k, num7])
                            {
                                num3 |= 1;
                            }
                            if (num4 == 8)
                            {
                                buffer[num2++] = (byte) num3;
                                num4 = 0;
                                num3 = 0;
                            }
                        }
                    }
                }
                flag = !flag;
            }
            if (num2 != version.TotalCodewords)
            {
                return null;
            }
            return buffer;
        }

        internal FormatInformation readFormatInformation()
        {
            if (this.parsedFormatInfo != null)
            {
                return this.parsedFormatInfo;
            }
            int versionBits = 0;
            for (int i = 0; i < 6; i++)
            {
                versionBits = this.copyBit(i, 8, versionBits);
            }
            versionBits = this.copyBit(7, 8, versionBits);
            versionBits = this.copyBit(8, 8, versionBits);
            versionBits = this.copyBit(8, 7, versionBits);
            for (int j = 5; j >= 0; j--)
            {
                versionBits = this.copyBit(8, j, versionBits);
            }
            int height = this.bitMatrix.Height;
            int num5 = 0;
            int num6 = height - 7;
            for (int k = height - 1; k >= num6; k--)
            {
                num5 = this.copyBit(8, k, num5);
            }
            for (int m = height - 8; m < height; m++)
            {
                num5 = this.copyBit(m, 8, num5);
            }
            this.parsedFormatInfo = FormatInformation.decodeFormatInformation(versionBits, num5);
            if (this.parsedFormatInfo != null)
            {
                return this.parsedFormatInfo;
            }
            return null;
        }

        internal Version readVersion()
        {
            if (this.parsedVersion != null)
            {
                return this.parsedVersion;
            }
            int height = this.bitMatrix.Height;
            int versionNumber = (height - 0x11) >> 2;
            if (versionNumber <= 6)
            {
                return Version.getVersionForNumber(versionNumber);
            }
            int versionBits = 0;
            int num4 = height - 11;
            for (int i = 5; i >= 0; i--)
            {
                for (int k = height - 9; k >= num4; k--)
                {
                    versionBits = this.copyBit(k, i, versionBits);
                }
            }
            this.parsedVersion = Version.decodeVersionInformation(versionBits);
            if ((this.parsedVersion != null) && (this.parsedVersion.DimensionForVersion == height))
            {
                return this.parsedVersion;
            }
            versionBits = 0;
            for (int j = 5; j >= 0; j--)
            {
                for (int m = height - 9; m >= num4; m--)
                {
                    versionBits = this.copyBit(j, m, versionBits);
                }
            }
            this.parsedVersion = Version.decodeVersionInformation(versionBits);
            if ((this.parsedVersion != null) && (this.parsedVersion.DimensionForVersion == height))
            {
                return this.parsedVersion;
            }
            return null;
        }

        internal void remask()
        {
            if (this.parsedFormatInfo != null)
            {
                DataMask mask = DataMask.forReference(this.parsedFormatInfo.DataMask);
                int height = this.bitMatrix.Height;
                mask.unmaskBitMatrix(this.bitMatrix, height);
            }
        }

        internal void setMirror(bool mirror)
        {
            this.parsedVersion = null;
            this.parsedFormatInfo = null;
            this.mirrored = mirror;
        }
    }
}

