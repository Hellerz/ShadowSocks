namespace ZXing.QrCode.Internal
{
    using System;
    using ZXing.Common;

    public sealed class Version
    {
        private readonly int[] alignmentPatternCenters;
        private readonly ECBlocks[] ecBlocks;
        private readonly int totalCodewords;
        private static readonly int[] VERSION_DECODE_INFO = new int[] { 
            0x7c94, 0x85bc, 0x9a99, 0xa4d3, 0xbbf6, 0xc762, 0xd847, 0xe60d, 0xf928, 0x10b78, 0x1145d, 0x12a17, 0x13532, 0x149a6, 0x15683, 0x168c9, 
            0x177ec, 0x18ec4, 0x191e1, 0x1afab, 0x1b08e, 0x1cc1a, 0x1d33f, 0x1ed75, 0x1f250, 0x209d5, 0x216f0, 0x228ba, 0x2379f, 0x24b0b, 0x2542e, 0x26a64, 
            0x27541, 0x28c69
         };
        private readonly int versionNumber;
        private static readonly Version[] VERSIONS = buildVersions();

        private Version(int versionNumber, int[] alignmentPatternCenters, params ECBlocks[] ecBlocks)
        {
            this.versionNumber = versionNumber;
            this.alignmentPatternCenters = alignmentPatternCenters;
            this.ecBlocks = ecBlocks;
            int num = 0;
            int eCCodewordsPerBlock = ecBlocks[0].ECCodewordsPerBlock;
            foreach (ECB ecb in ecBlocks[0].getECBlocks())
            {
                num += ecb.Count * (ecb.DataCodewords + eCCodewordsPerBlock);
            }
            this.totalCodewords = num;
        }

        internal BitMatrix buildFunctionPattern()
        {
            int dimensionForVersion = this.DimensionForVersion;
            BitMatrix matrix = new BitMatrix(dimensionForVersion);
            matrix.setRegion(0, 0, 9, 9);
            matrix.setRegion(dimensionForVersion - 8, 0, 8, 9);
            matrix.setRegion(0, dimensionForVersion - 8, 9, 8);
            int length = this.alignmentPatternCenters.Length;
            for (int i = 0; i < length; i++)
            {
                int top = this.alignmentPatternCenters[i] - 2;
                for (int j = 0; j < length; j++)
                {
                    if (((i != 0) || ((j != 0) && (j != (length - 1)))) && ((i != (length - 1)) || (j != 0)))
                    {
                        matrix.setRegion(this.alignmentPatternCenters[j] - 2, top, 5, 5);
                    }
                }
            }
            matrix.setRegion(6, 9, 1, dimensionForVersion - 0x11);
            matrix.setRegion(9, 6, dimensionForVersion - 0x11, 1);
            if (this.versionNumber > 6)
            {
                matrix.setRegion(dimensionForVersion - 11, 0, 3, 6);
                matrix.setRegion(0, dimensionForVersion - 11, 6, 3);
            }
            return matrix;
        }

        private static Version[] buildVersions()
        {
            return new Version[] { 
                new Version(1, new int[0], new ECBlocks[] { new ECBlocks(7, new ECB[] { new ECB(1, 0x13) }), new ECBlocks(10, new ECB[] { new ECB(1, 0x10) }), new ECBlocks(13, new ECB[] { new ECB(1, 13) }), new ECBlocks(0x11, new ECB[] { new ECB(1, 9) }) }), new Version(2, new int[] { 6, 0x12 }, new ECBlocks[] { new ECBlocks(10, new ECB[] { new ECB(1, 0x22) }), new ECBlocks(0x10, new ECB[] { new ECB(1, 0x1c) }), new ECBlocks(0x16, new ECB[] { new ECB(1, 0x16) }), new ECBlocks(0x1c, new ECB[] { new ECB(1, 0x10) }) }), new Version(3, new int[] { 6, 0x16 }, new ECBlocks[] { new ECBlocks(15, new ECB[] { new ECB(1, 0x37) }), new ECBlocks(0x1a, new ECB[] { new ECB(1, 0x2c) }), new ECBlocks(0x12, new ECB[] { new ECB(2, 0x11) }), new ECBlocks(0x16, new ECB[] { new ECB(2, 13) }) }), new Version(4, new int[] { 6, 0x1a }, new ECBlocks[] { new ECBlocks(20, new ECB[] { new ECB(1, 80) }), new ECBlocks(0x12, new ECB[] { new ECB(2, 0x20) }), new ECBlocks(0x1a, new ECB[] { new ECB(2, 0x18) }), new ECBlocks(0x10, new ECB[] { new ECB(4, 9) }) }), new Version(5, new int[] { 6, 30 }, new ECBlocks[] { new ECBlocks(0x1a, new ECB[] { new ECB(1, 0x6c) }), new ECBlocks(0x18, new ECB[] { new ECB(2, 0x2b) }), new ECBlocks(0x12, new ECB[] { new ECB(2, 15), new ECB(2, 0x10) }), new ECBlocks(0x16, new ECB[] { new ECB(2, 11), new ECB(2, 12) }) }), new Version(6, new int[] { 6, 0x22 }, new ECBlocks[] { new ECBlocks(0x12, new ECB[] { new ECB(2, 0x44) }), new ECBlocks(0x10, new ECB[] { new ECB(4, 0x1b) }), new ECBlocks(0x18, new ECB[] { new ECB(4, 0x13) }), new ECBlocks(0x1c, new ECB[] { new ECB(4, 15) }) }), new Version(7, new int[] { 6, 0x16, 0x26 }, new ECBlocks[] { new ECBlocks(20, new ECB[] { new ECB(2, 0x4e) }), new ECBlocks(0x12, new ECB[] { new ECB(4, 0x1f) }), new ECBlocks(0x12, new ECB[] { new ECB(2, 14), new ECB(4, 15) }), new ECBlocks(0x1a, new ECB[] { new ECB(4, 13), new ECB(1, 14) }) }), new Version(8, new int[] { 6, 0x18, 0x2a }, new ECBlocks[] { new ECBlocks(0x18, new ECB[] { new ECB(2, 0x61) }), new ECBlocks(0x16, new ECB[] { new ECB(2, 0x26), new ECB(2, 0x27) }), new ECBlocks(0x16, new ECB[] { new ECB(4, 0x12), new ECB(2, 0x13) }), new ECBlocks(0x1a, new ECB[] { new ECB(4, 14), new ECB(2, 15) }) }), new Version(9, new int[] { 6, 0x1a, 0x2e }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(2, 0x74) }), new ECBlocks(0x16, new ECB[] { new ECB(3, 0x24), new ECB(2, 0x25) }), new ECBlocks(20, new ECB[] { new ECB(4, 0x10), new ECB(4, 0x11) }), new ECBlocks(0x18, new ECB[] { new ECB(4, 12), new ECB(4, 13) }) }), new Version(10, new int[] { 6, 0x1c, 50 }, new ECBlocks[] { new ECBlocks(0x12, new ECB[] { new ECB(2, 0x44), new ECB(2, 0x45) }), new ECBlocks(0x1a, new ECB[] { new ECB(4, 0x2b), new ECB(1, 0x2c) }), new ECBlocks(0x18, new ECB[] { new ECB(6, 0x13), new ECB(2, 20) }), new ECBlocks(0x1c, new ECB[] { new ECB(6, 15), new ECB(2, 0x10) }) }), new Version(11, new int[] { 6, 30, 0x36 }, new ECBlocks[] { new ECBlocks(20, new ECB[] { new ECB(4, 0x51) }), new ECBlocks(30, new ECB[] { new ECB(1, 50), new ECB(4, 0x33) }), new ECBlocks(0x1c, new ECB[] { new ECB(4, 0x16), new ECB(4, 0x17) }), new ECBlocks(0x18, new ECB[] { new ECB(3, 12), new ECB(8, 13) }) }), new Version(12, new int[] { 6, 0x20, 0x3a }, new ECBlocks[] { new ECBlocks(0x18, new ECB[] { new ECB(2, 0x5c), new ECB(2, 0x5d) }), new ECBlocks(0x16, new ECB[] { new ECB(6, 0x24), new ECB(2, 0x25) }), new ECBlocks(0x1a, new ECB[] { new ECB(4, 20), new ECB(6, 0x15) }), new ECBlocks(0x1c, new ECB[] { new ECB(7, 14), new ECB(4, 15) }) }), new Version(13, new int[] { 6, 0x22, 0x3e }, new ECBlocks[] { new ECBlocks(0x1a, new ECB[] { new ECB(4, 0x6b) }), new ECBlocks(0x16, new ECB[] { new ECB(8, 0x25), new ECB(1, 0x26) }), new ECBlocks(0x18, new ECB[] { new ECB(8, 20), new ECB(4, 0x15) }), new ECBlocks(0x16, new ECB[] { new ECB(12, 11), new ECB(4, 12) }) }), new Version(14, new int[] { 6, 0x1a, 0x2e, 0x42 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(3, 0x73), new ECB(1, 0x74) }), new ECBlocks(0x18, new ECB[] { new ECB(4, 40), new ECB(5, 0x29) }), new ECBlocks(20, new ECB[] { new ECB(11, 0x10), new ECB(5, 0x11) }), new ECBlocks(0x18, new ECB[] { new ECB(11, 12), new ECB(5, 13) }) }), new Version(15, new int[] { 6, 0x1a, 0x30, 70 }, new ECBlocks[] { new ECBlocks(0x16, new ECB[] { new ECB(5, 0x57), new ECB(1, 0x58) }), new ECBlocks(0x18, new ECB[] { new ECB(5, 0x29), new ECB(5, 0x2a) }), new ECBlocks(30, new ECB[] { new ECB(5, 0x18), new ECB(7, 0x19) }), new ECBlocks(0x18, new ECB[] { new ECB(11, 12), new ECB(7, 13) }) }), new Version(0x10, new int[] { 6, 0x1a, 50, 0x4a }, new ECBlocks[] { new ECBlocks(0x18, new ECB[] { new ECB(5, 0x62), new ECB(1, 0x63) }), new ECBlocks(0x1c, new ECB[] { new ECB(7, 0x2d), new ECB(3, 0x2e) }), new ECBlocks(0x18, new ECB[] { new ECB(15, 0x13), new ECB(2, 20) }), new ECBlocks(30, new ECB[] { new ECB(3, 15), new ECB(13, 0x10) }) }), 
                new Version(0x11, new int[] { 6, 30, 0x36, 0x4e }, new ECBlocks[] { new ECBlocks(0x1c, new ECB[] { new ECB(1, 0x6b), new ECB(5, 0x6c) }), new ECBlocks(0x1c, new ECB[] { new ECB(10, 0x2e), new ECB(1, 0x2f) }), new ECBlocks(0x1c, new ECB[] { new ECB(1, 0x16), new ECB(15, 0x17) }), new ECBlocks(0x1c, new ECB[] { new ECB(2, 14), new ECB(0x11, 15) }) }), new Version(0x12, new int[] { 6, 30, 0x38, 0x52 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(5, 120), new ECB(1, 0x79) }), new ECBlocks(0x1a, new ECB[] { new ECB(9, 0x2b), new ECB(4, 0x2c) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x11, 0x16), new ECB(1, 0x17) }), new ECBlocks(0x1c, new ECB[] { new ECB(2, 14), new ECB(0x13, 15) }) }), new Version(0x13, new int[] { 6, 30, 0x3a, 0x56 }, new ECBlocks[] { new ECBlocks(0x1c, new ECB[] { new ECB(3, 0x71), new ECB(4, 0x72) }), new ECBlocks(0x1a, new ECB[] { new ECB(3, 0x2c), new ECB(11, 0x2d) }), new ECBlocks(0x1a, new ECB[] { new ECB(0x11, 0x15), new ECB(4, 0x16) }), new ECBlocks(0x1a, new ECB[] { new ECB(9, 13), new ECB(0x10, 14) }) }), new Version(20, new int[] { 6, 0x22, 0x3e, 90 }, new ECBlocks[] { new ECBlocks(0x1c, new ECB[] { new ECB(3, 0x6b), new ECB(5, 0x6c) }), new ECBlocks(0x1a, new ECB[] { new ECB(3, 0x29), new ECB(13, 0x2a) }), new ECBlocks(30, new ECB[] { new ECB(15, 0x18), new ECB(5, 0x19) }), new ECBlocks(0x1c, new ECB[] { new ECB(15, 15), new ECB(10, 0x10) }) }), new Version(0x15, new int[] { 6, 0x1c, 50, 0x48, 0x5e }, new ECBlocks[] { new ECBlocks(0x1c, new ECB[] { new ECB(4, 0x74), new ECB(4, 0x75) }), new ECBlocks(0x1a, new ECB[] { new ECB(0x11, 0x2a) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x11, 0x16), new ECB(6, 0x17) }), new ECBlocks(30, new ECB[] { new ECB(0x13, 0x10), new ECB(6, 0x11) }) }), new Version(0x16, new int[] { 6, 0x1a, 50, 0x4a, 0x62 }, new ECBlocks[] { new ECBlocks(0x1c, new ECB[] { new ECB(2, 0x6f), new ECB(7, 0x70) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x11, 0x2e) }), new ECBlocks(30, new ECB[] { new ECB(7, 0x18), new ECB(0x10, 0x19) }), new ECBlocks(0x18, new ECB[] { new ECB(0x22, 13) }) }), new Version(0x17, new int[] { 6, 30, 0x36, 0x4a, 0x66 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(4, 0x79), new ECB(5, 0x7a) }), new ECBlocks(0x1c, new ECB[] { new ECB(4, 0x2f), new ECB(14, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(11, 0x18), new ECB(14, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x10, 15), new ECB(14, 0x10) }) }), new Version(0x18, new int[] { 6, 0x1c, 0x36, 80, 0x6a }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(6, 0x75), new ECB(4, 0x76) }), new ECBlocks(0x1c, new ECB[] { new ECB(6, 0x2d), new ECB(14, 0x2e) }), new ECBlocks(30, new ECB[] { new ECB(11, 0x18), new ECB(0x10, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(30, 0x10), new ECB(2, 0x11) }) }), new Version(0x19, new int[] { 6, 0x20, 0x3a, 0x54, 110 }, new ECBlocks[] { new ECBlocks(0x1a, new ECB[] { new ECB(8, 0x6a), new ECB(4, 0x6b) }), new ECBlocks(0x1c, new ECB[] { new ECB(8, 0x2f), new ECB(13, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(7, 0x18), new ECB(0x16, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x16, 15), new ECB(13, 0x10) }) }), new Version(0x1a, new int[] { 6, 30, 0x3a, 0x56, 0x72 }, new ECBlocks[] { new ECBlocks(0x1c, new ECB[] { new ECB(10, 0x72), new ECB(2, 0x73) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x13, 0x2e), new ECB(4, 0x2f) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x1c, 0x16), new ECB(6, 0x17) }), new ECBlocks(30, new ECB[] { new ECB(0x21, 0x10), new ECB(4, 0x11) }) }), new Version(0x1b, new int[] { 6, 0x22, 0x3e, 90, 0x76 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(8, 0x7a), new ECB(4, 0x7b) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x16, 0x2d), new ECB(3, 0x2e) }), new ECBlocks(30, new ECB[] { new ECB(8, 0x17), new ECB(0x1a, 0x18) }), new ECBlocks(30, new ECB[] { new ECB(12, 15), new ECB(0x1c, 0x10) }) }), new Version(0x1c, new int[] { 6, 0x1a, 50, 0x4a, 0x62, 0x7a }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(3, 0x75), new ECB(10, 0x76) }), new ECBlocks(0x1c, new ECB[] { new ECB(3, 0x2d), new ECB(0x17, 0x2e) }), new ECBlocks(30, new ECB[] { new ECB(4, 0x18), new ECB(0x1f, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(11, 15), new ECB(0x1f, 0x10) }) }), new Version(0x1d, new int[] { 6, 30, 0x36, 0x4e, 0x66, 0x7e }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(7, 0x74), new ECB(7, 0x75) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x15, 0x2d), new ECB(7, 0x2e) }), new ECBlocks(30, new ECB[] { new ECB(1, 0x17), new ECB(0x25, 0x18) }), new ECBlocks(30, new ECB[] { new ECB(0x13, 15), new ECB(0x1a, 0x10) }) }), new Version(30, new int[] { 6, 0x1a, 0x34, 0x4e, 0x68, 130 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(5, 0x73), new ECB(10, 0x74) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x13, 0x2f), new ECB(10, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(15, 0x18), new ECB(0x19, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x17, 15), new ECB(0x19, 0x10) }) }), new Version(0x1f, new int[] { 6, 30, 0x38, 0x52, 0x6c, 0x86 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(13, 0x73), new ECB(3, 0x74) }), new ECBlocks(0x1c, new ECB[] { new ECB(2, 0x2e), new ECB(0x1d, 0x2f) }), new ECBlocks(30, new ECB[] { new ECB(0x2a, 0x18), new ECB(1, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x17, 15), new ECB(0x1c, 0x10) }) }), new Version(0x20, new int[] { 6, 0x22, 60, 0x56, 0x70, 0x8a }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(0x11, 0x73) }), new ECBlocks(0x1c, new ECB[] { new ECB(10, 0x2e), new ECB(0x17, 0x2f) }), new ECBlocks(30, new ECB[] { new ECB(10, 0x18), new ECB(0x23, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x13, 15), new ECB(0x23, 0x10) }) }), 
                new Version(0x21, new int[] { 6, 30, 0x3a, 0x56, 0x72, 0x8e }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(0x11, 0x73), new ECB(1, 0x74) }), new ECBlocks(0x1c, new ECB[] { new ECB(14, 0x2e), new ECB(0x15, 0x2f) }), new ECBlocks(30, new ECB[] { new ECB(0x1d, 0x18), new ECB(0x13, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(11, 15), new ECB(0x2e, 0x10) }) }), new Version(0x22, new int[] { 6, 0x22, 0x3e, 90, 0x76, 0x92 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(13, 0x73), new ECB(6, 0x74) }), new ECBlocks(0x1c, new ECB[] { new ECB(14, 0x2e), new ECB(0x17, 0x2f) }), new ECBlocks(30, new ECB[] { new ECB(0x2c, 0x18), new ECB(7, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x3b, 0x10), new ECB(1, 0x11) }) }), new Version(0x23, new int[] { 6, 30, 0x36, 0x4e, 0x66, 0x7e, 150 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(12, 0x79), new ECB(7, 0x7a) }), new ECBlocks(0x1c, new ECB[] { new ECB(12, 0x2f), new ECB(0x1a, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(0x27, 0x18), new ECB(14, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x16, 15), new ECB(0x29, 0x10) }) }), new Version(0x24, new int[] { 6, 0x18, 50, 0x4c, 0x66, 0x80, 0x9a }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(6, 0x79), new ECB(14, 0x7a) }), new ECBlocks(0x1c, new ECB[] { new ECB(6, 0x2f), new ECB(0x22, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(0x2e, 0x18), new ECB(10, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(2, 15), new ECB(0x40, 0x10) }) }), new Version(0x25, new int[] { 6, 0x1c, 0x36, 80, 0x6a, 0x84, 0x9e }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(0x11, 0x7a), new ECB(4, 0x7b) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x1d, 0x2e), new ECB(14, 0x2f) }), new ECBlocks(30, new ECB[] { new ECB(0x31, 0x18), new ECB(10, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x18, 15), new ECB(0x2e, 0x10) }) }), new Version(0x26, new int[] { 6, 0x20, 0x3a, 0x54, 110, 0x88, 0xa2 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(4, 0x7a), new ECB(0x12, 0x7b) }), new ECBlocks(0x1c, new ECB[] { new ECB(13, 0x2e), new ECB(0x20, 0x2f) }), new ECBlocks(30, new ECB[] { new ECB(0x30, 0x18), new ECB(14, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(0x2a, 15), new ECB(0x20, 0x10) }) }), new Version(0x27, new int[] { 6, 0x1a, 0x36, 0x52, 110, 0x8a, 0xa6 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(20, 0x75), new ECB(4, 0x76) }), new ECBlocks(0x1c, new ECB[] { new ECB(40, 0x2f), new ECB(7, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(0x2b, 0x18), new ECB(0x16, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(10, 15), new ECB(0x43, 0x10) }) }), new Version(40, new int[] { 6, 30, 0x3a, 0x56, 0x72, 0x8e, 170 }, new ECBlocks[] { new ECBlocks(30, new ECB[] { new ECB(0x13, 0x76), new ECB(6, 0x77) }), new ECBlocks(0x1c, new ECB[] { new ECB(0x12, 0x2f), new ECB(0x1f, 0x30) }), new ECBlocks(30, new ECB[] { new ECB(0x22, 0x18), new ECB(0x22, 0x19) }), new ECBlocks(30, new ECB[] { new ECB(20, 15), new ECB(0x3d, 0x10) }) })
             };
        }

        internal static Version decodeVersionInformation(int versionBits)
        {
            int num = 0x7fffffff;
            int versionNumber = 0;
            for (int i = 0; i < VERSION_DECODE_INFO.Length; i++)
            {
                int b = VERSION_DECODE_INFO[i];
                if (b == versionBits)
                {
                    return getVersionForNumber(i + 7);
                }
                int num5 = FormatInformation.numBitsDiffering(versionBits, b);
                if (num5 < num)
                {
                    versionNumber = i + 7;
                    num = num5;
                }
            }
            if (num <= 3)
            {
                return getVersionForNumber(versionNumber);
            }
            return null;
        }

        public ECBlocks getECBlocksForLevel(ErrorCorrectionLevel ecLevel)
        {
            return this.ecBlocks[ecLevel.ordinal()];
        }

        public static Version getProvisionalVersionForDimension(int dimension)
        {
            if ((dimension % 4) != 1)
            {
                return null;
            }
            try
            {
                return getVersionForNumber((dimension - 0x11) >> 2);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static Version getVersionForNumber(int versionNumber)
        {
            if ((versionNumber < 1) || (versionNumber > 40))
            {
                throw new ArgumentException();
            }
            return VERSIONS[versionNumber - 1];
        }

        public override string ToString()
        {
            return Convert.ToString(this.versionNumber);
        }

        public int[] AlignmentPatternCenters
        {
            get
            {
                return this.alignmentPatternCenters;
            }
        }

        public int DimensionForVersion
        {
            get
            {
                return (0x11 + (4 * this.versionNumber));
            }
        }

        public int TotalCodewords
        {
            get
            {
                return this.totalCodewords;
            }
        }

        public int VersionNumber
        {
            get
            {
                return this.versionNumber;
            }
        }

        public sealed class ECB
        {
            private readonly int count;
            private readonly int dataCodewords;

            internal ECB(int count, int dataCodewords)
            {
                this.count = count;
                this.dataCodewords = dataCodewords;
            }

            public int Count
            {
                get
                {
                    return this.count;
                }
            }

            public int DataCodewords
            {
                get
                {
                    return this.dataCodewords;
                }
            }
        }

        public sealed class ECBlocks
        {
            private readonly Version.ECB[] ecBlocks;
            private readonly int ecCodewordsPerBlock;

            internal ECBlocks(int ecCodewordsPerBlock, params Version.ECB[] ecBlocks)
            {
                this.ecCodewordsPerBlock = ecCodewordsPerBlock;
                this.ecBlocks = ecBlocks;
            }

            public Version.ECB[] getECBlocks()
            {
                return this.ecBlocks;
            }

            public int ECCodewordsPerBlock
            {
                get
                {
                    return this.ecCodewordsPerBlock;
                }
            }

            public int NumBlocks
            {
                get
                {
                    int num = 0;
                    foreach (Version.ECB ecb in this.ecBlocks)
                    {
                        num += ecb.Count;
                    }
                    return num;
                }
            }

            public int TotalECCodewords
            {
                get
                {
                    return (this.ecCodewordsPerBlock * this.NumBlocks);
                }
            }
        }
    }
}

