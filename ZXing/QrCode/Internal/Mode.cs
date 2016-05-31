namespace ZXing.QrCode.Internal
{
    using System;

    public sealed class Mode
    {
        public static readonly Mode ALPHANUMERIC;
        private readonly int bits;
        public static readonly Mode BYTE;
        private readonly int[] characterCountBitsForVersions;
        public static readonly Mode ECI;
        public static readonly Mode FNC1_FIRST_POSITION;
        public static readonly Mode FNC1_SECOND_POSITION;
        public static readonly Mode HANZI;
        public static readonly Mode KANJI;
        private readonly string name;
        public static readonly Mode NUMERIC;
        public static readonly Mode STRUCTURED_APPEND;
        public static readonly Mode TERMINATOR;

        static Mode()
        {
            int[] characterCountBitsForVersions = new int[3];
            TERMINATOR = new Mode(characterCountBitsForVersions, 0, "TERMINATOR");
            NUMERIC = new Mode(new int[] { 10, 12, 14 }, 1, "NUMERIC");
            ALPHANUMERIC = new Mode(new int[] { 9, 11, 13 }, 2, "ALPHANUMERIC");
            int[] numArray2 = new int[3];
            STRUCTURED_APPEND = new Mode(numArray2, 3, "STRUCTURED_APPEND");
            BYTE = new Mode(new int[] { 8, 0x10, 0x10 }, 4, "BYTE");
            ECI = new Mode(null, 7, "ECI");
            KANJI = new Mode(new int[] { 8, 10, 12 }, 8, "KANJI");
            FNC1_FIRST_POSITION = new Mode(null, 5, "FNC1_FIRST_POSITION");
            FNC1_SECOND_POSITION = new Mode(null, 9, "FNC1_SECOND_POSITION");
            HANZI = new Mode(new int[] { 8, 10, 12 }, 13, "HANZI");
        }

        private Mode(int[] characterCountBitsForVersions, int bits, string name)
        {
            this.characterCountBitsForVersions = characterCountBitsForVersions;
            this.bits = bits;
            this.name = name;
        }

        public static Mode forBits(int bits)
        {
            switch (bits)
            {
                case 0:
                    return TERMINATOR;

                case 1:
                    return NUMERIC;

                case 2:
                    return ALPHANUMERIC;

                case 3:
                    return STRUCTURED_APPEND;

                case 4:
                    return BYTE;

                case 5:
                    return FNC1_FIRST_POSITION;

                case 7:
                    return ECI;

                case 8:
                    return KANJI;

                case 9:
                    return FNC1_SECOND_POSITION;

                case 13:
                    return HANZI;
            }
            throw new ArgumentException();
        }

        public int getCharacterCountBits(Version version)
        {
            int num2;
            if (this.characterCountBitsForVersions == null)
            {
                throw new ArgumentException("Character count doesn't apply to this mode");
            }
            int versionNumber = version.VersionNumber;
            if (versionNumber <= 9)
            {
                num2 = 0;
            }
            else if (versionNumber <= 0x1a)
            {
                num2 = 1;
            }
            else
            {
                num2 = 2;
            }
            return this.characterCountBitsForVersions[num2];
        }

        public override string ToString()
        {
            return this.name;
        }

        public int Bits
        {
            get
            {
                return this.bits;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

