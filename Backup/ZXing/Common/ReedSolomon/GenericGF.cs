namespace ZXing.Common.ReedSolomon
{
    using System;

    public sealed class GenericGF
    {
        public static GenericGF AZTEC_DATA_10 = new GenericGF(0x409, 0x400, 1);
        public static GenericGF AZTEC_DATA_12 = new GenericGF(0x1069, 0x1000, 1);
        public static GenericGF AZTEC_DATA_6 = new GenericGF(0x43, 0x40, 1);
        public static GenericGF AZTEC_DATA_8 = DATA_MATRIX_FIELD_256;
        public static GenericGF AZTEC_PARAM = new GenericGF(0x13, 0x10, 1);
        public static GenericGF DATA_MATRIX_FIELD_256 = new GenericGF(0x12d, 0x100, 1);
        private int[] expTable;
        private readonly int generatorBase;
        private const int INITIALIZATION_THRESHOLD = 0;
        private bool initialized;
        private int[] logTable;
        public static GenericGF MAXICODE_FIELD_64 = AZTEC_DATA_6;
        private GenericGFPoly one;
        private readonly int primitive;
        public static GenericGF QR_CODE_FIELD_256 = new GenericGF(0x11d, 0x100, 0);
        private readonly int size;
        private GenericGFPoly zero;

        public GenericGF(int primitive, int size, int genBase)
        {
            this.primitive = primitive;
            this.size = size;
            this.generatorBase = genBase;
            if (size <= 0)
            {
                this.initialize();
            }
        }

        internal static int addOrSubtract(int a, int b)
        {
            return (a ^ b);
        }

        internal GenericGFPoly buildMonomial(int degree, int coefficient)
        {
            this.checkInit();
            if (degree < 0)
            {
                throw new ArgumentException();
            }
            if (coefficient == 0)
            {
                return this.zero;
            }
            int[] coefficients = new int[degree + 1];
            coefficients[0] = coefficient;
            return new GenericGFPoly(this, coefficients);
        }

        private void checkInit()
        {
            if (!this.initialized)
            {
                this.initialize();
            }
        }

        internal int exp(int a)
        {
            this.checkInit();
            return this.expTable[a];
        }

        private void initialize()
        {
            this.expTable = new int[this.size];
            this.logTable = new int[this.size];
            int num = 1;
            for (int i = 0; i < this.size; i++)
            {
                this.expTable[i] = num;
                num = num << 1;
                if (num >= this.size)
                {
                    num ^= this.primitive;
                    num &= this.size - 1;
                }
            }
            for (int j = 0; j < (this.size - 1); j++)
            {
                this.logTable[this.expTable[j]] = j;
            }
            int[] coefficients = new int[1];
            this.zero = new GenericGFPoly(this, coefficients);
            this.one = new GenericGFPoly(this, new int[] { 1 });
            this.initialized = true;
        }

        internal int inverse(int a)
        {
            this.checkInit();
            if (a == 0)
            {
                throw new ArithmeticException();
            }
            return this.expTable[(this.size - this.logTable[a]) - 1];
        }

        internal int log(int a)
        {
            this.checkInit();
            if (a == 0)
            {
                throw new ArgumentException();
            }
            return this.logTable[a];
        }

        internal int multiply(int a, int b)
        {
            this.checkInit();
            if ((a != 0) && (b != 0))
            {
                return this.expTable[(this.logTable[a] + this.logTable[b]) % (this.size - 1)];
            }
            return 0;
        }

        public override string ToString()
        {
            return string.Concat(new object[] { "GF(0x", this.primitive.ToString("X"), ',', this.size, ')' });
        }

        public int GeneratorBase
        {
            get
            {
                return this.generatorBase;
            }
        }

        internal GenericGFPoly One
        {
            get
            {
                this.checkInit();
                return this.one;
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
        }

        internal GenericGFPoly Zero
        {
            get
            {
                this.checkInit();
                return this.zero;
            }
        }
    }
}

