namespace ZXing.Common.ReedSolomon
{
    using System;
    using System.Collections.Generic;

    public sealed class ReedSolomonEncoder
    {
        private readonly IList<GenericGFPoly> cachedGenerators;
        private readonly GenericGF field;

        public ReedSolomonEncoder(GenericGF field)
        {
            this.field = field;
            this.cachedGenerators = new List<GenericGFPoly>();
            this.cachedGenerators.Add(new GenericGFPoly(field, new int[] { 1 }));
        }

        private GenericGFPoly buildGenerator(int degree)
        {
            if (degree >= this.cachedGenerators.Count)
            {
                GenericGFPoly poly = this.cachedGenerators[this.cachedGenerators.Count - 1];
                for (int i = this.cachedGenerators.Count; i <= degree; i++)
                {
                    GenericGFPoly item = poly.multiply(new GenericGFPoly(this.field, new int[] { 1, this.field.exp((i - 1) + this.field.GeneratorBase) }));
                    this.cachedGenerators.Add(item);
                    poly = item;
                }
            }
            return this.cachedGenerators[degree];
        }

        public void encode(int[] toEncode, int ecBytes)
        {
            if (ecBytes == 0)
            {
                throw new ArgumentException("No error correction bytes");
            }
            int length = toEncode.Length - ecBytes;
            if (length <= 0)
            {
                throw new ArgumentException("No data bytes provided");
            }
            GenericGFPoly other = this.buildGenerator(ecBytes);
            int[] destinationArray = new int[length];
            Array.Copy(toEncode, 0, destinationArray, 0, length);
            GenericGFPoly poly2 = new GenericGFPoly(this.field, destinationArray);
            GenericGFPoly poly3 = poly2.multiplyByMonomial(ecBytes, 1).divide(other)[1];
            int[] coefficients = poly3.Coefficients;
            int num2 = ecBytes - coefficients.Length;
            for (int i = 0; i < num2; i++)
            {
                toEncode[length + i] = 0;
            }
            Array.Copy(coefficients, 0, toEncode, length + num2, coefficients.Length);
        }
    }
}

