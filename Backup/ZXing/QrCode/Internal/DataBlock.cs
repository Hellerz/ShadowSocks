namespace ZXing.QrCode.Internal
{
    using System;

    internal sealed class DataBlock
    {
        private readonly byte[] codewords;
        private readonly int numDataCodewords;

        private DataBlock(int numDataCodewords, byte[] codewords)
        {
            this.numDataCodewords = numDataCodewords;
            this.codewords = codewords;
        }

        internal static DataBlock[] getDataBlocks(byte[] rawCodewords, Version version, ErrorCorrectionLevel ecLevel)
        {
            if (rawCodewords.Length != version.TotalCodewords)
            {
                throw new ArgumentException();
            }
            Version.ECBlocks blocks = version.getECBlocksForLevel(ecLevel);
            int num = 0;
            Version.ECB[] ecbArray = blocks.getECBlocks();
            foreach (Version.ECB ecb in ecbArray)
            {
                num += ecb.Count;
            }
            DataBlock[] blockArray = new DataBlock[num];
            int num2 = 0;
            foreach (Version.ECB ecb2 in ecbArray)
            {
                for (int m = 0; m < ecb2.Count; m++)
                {
                    int dataCodewords = ecb2.DataCodewords;
                    int num5 = blocks.ECCodewordsPerBlock + dataCodewords;
                    blockArray[num2++] = new DataBlock(dataCodewords, new byte[num5]);
                }
            }
            int length = blockArray[0].codewords.Length;
            int index = blockArray.Length - 1;
            while (index >= 0)
            {
                if (blockArray[index].codewords.Length == length)
                {
                    break;
                }
                index--;
            }
            index++;
            int num9 = length - blocks.ECCodewordsPerBlock;
            int num10 = 0;
            for (int i = 0; i < num9; i++)
            {
                for (int n = 0; n < num2; n++)
                {
                    blockArray[n].codewords[i] = rawCodewords[num10++];
                }
            }
            for (int j = index; j < num2; j++)
            {
                blockArray[j].codewords[num9] = rawCodewords[num10++];
            }
            int num14 = blockArray[0].codewords.Length;
            for (int k = num9; k < num14; k++)
            {
                for (int num16 = 0; num16 < num2; num16++)
                {
                    int num17 = (num16 < index) ? k : (k + 1);
                    blockArray[num16].codewords[num17] = rawCodewords[num10++];
                }
            }
            return blockArray;
        }

        internal byte[] Codewords
        {
            get
            {
                return this.codewords;
            }
        }

        internal int NumDataCodewords
        {
            get
            {
                return this.numDataCodewords;
            }
        }
    }
}

