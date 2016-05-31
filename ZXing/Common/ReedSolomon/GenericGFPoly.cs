namespace ZXing.Common.ReedSolomon
{
    using System;
    using System.Text;

    internal sealed class GenericGFPoly
    {
        private readonly int[] coefficients;
        private readonly GenericGF field;

        internal GenericGFPoly(GenericGF field, int[] coefficients)
        {
            if (coefficients.Length == 0)
            {
                throw new ArgumentException();
            }
            this.field = field;
            int length = coefficients.Length;
            if ((length > 1) && (coefficients[0] == 0))
            {
                int index = 1;
                while ((index < length) && (coefficients[index] == 0))
                {
                    index++;
                }
                if (index == length)
                {
                    this.coefficients = field.Zero.coefficients;
                }
                else
                {
                    this.coefficients = new int[length - index];
                    Array.Copy(coefficients, index, this.coefficients, 0, this.coefficients.Length);
                }
            }
            else
            {
                this.coefficients = coefficients;
            }
        }

        internal GenericGFPoly addOrSubtract(GenericGFPoly other)
        {
            if (!this.field.Equals(other.field))
            {
                throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
            }
            if (this.isZero)
            {
                return other;
            }
            if (other.isZero)
            {
                return this;
            }
            int[] coefficients = this.coefficients;
            int[] sourceArray = other.coefficients;
            if (coefficients.Length > sourceArray.Length)
            {
                int[] numArray3 = coefficients;
                coefficients = sourceArray;
                sourceArray = numArray3;
            }
            int[] destinationArray = new int[sourceArray.Length];
            int length = sourceArray.Length - coefficients.Length;
            Array.Copy(sourceArray, 0, destinationArray, 0, length);
            for (int i = length; i < sourceArray.Length; i++)
            {
                destinationArray[i] = GenericGF.addOrSubtract(coefficients[i - length], sourceArray[i]);
            }
            return new GenericGFPoly(this.field, destinationArray);
        }

        internal GenericGFPoly[] divide(GenericGFPoly other)
        {
            if (!this.field.Equals(other.field))
            {
                throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
            }
            if (other.isZero)
            {
                throw new ArgumentException("Divide by 0");
            }
            GenericGFPoly zero = this.field.Zero;
            GenericGFPoly poly2 = this;
            int a = other.getCoefficient(other.Degree);
            int b = this.field.inverse(a);
            while ((poly2.Degree >= other.Degree) && !poly2.isZero)
            {
                int degree = poly2.Degree - other.Degree;
                int coefficient = this.field.multiply(poly2.getCoefficient(poly2.Degree), b);
                GenericGFPoly poly3 = other.multiplyByMonomial(degree, coefficient);
                GenericGFPoly poly4 = this.field.buildMonomial(degree, coefficient);
                zero = zero.addOrSubtract(poly4);
                poly2 = poly2.addOrSubtract(poly3);
            }
            return new GenericGFPoly[] { zero, poly2 };
        }

        internal int evaluateAt(int a)
        {
            int num = 0;
            if (a == 0)
            {
                return this.getCoefficient(0);
            }
            int length = this.coefficients.Length;
            if (a == 1)
            {
                foreach (int num3 in this.coefficients)
                {
                    num = GenericGF.addOrSubtract(num, num3);
                }
                return num;
            }
            num = this.coefficients[0];
            for (int i = 1; i < length; i++)
            {
                num = GenericGF.addOrSubtract(this.field.multiply(a, num), this.coefficients[i]);
            }
            return num;
        }

        internal int getCoefficient(int degree)
        {
            return this.coefficients[(this.coefficients.Length - 1) - degree];
        }

        internal GenericGFPoly multiply(int scalar)
        {
            if (scalar == 0)
            {
                return this.field.Zero;
            }
            if (scalar == 1)
            {
                return this;
            }
            int length = this.coefficients.Length;
            int[] coefficients = new int[length];
            for (int i = 0; i < length; i++)
            {
                coefficients[i] = this.field.multiply(this.coefficients[i], scalar);
            }
            return new GenericGFPoly(this.field, coefficients);
        }

        internal GenericGFPoly multiply(GenericGFPoly other)
        {
            if (!this.field.Equals(other.field))
            {
                throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
            }
            if (this.isZero || other.isZero)
            {
                return this.field.Zero;
            }
            int[] coefficients = this.coefficients;
            int length = coefficients.Length;
            int[] numArray2 = other.coefficients;
            int num2 = numArray2.Length;
            int[] numArray3 = new int[(length + num2) - 1];
            for (int i = 0; i < length; i++)
            {
                int a = coefficients[i];
                for (int j = 0; j < num2; j++)
                {
                    numArray3[i + j] = GenericGF.addOrSubtract(numArray3[i + j], this.field.multiply(a, numArray2[j]));
                }
            }
            return new GenericGFPoly(this.field, numArray3);
        }

        internal GenericGFPoly multiplyByMonomial(int degree, int coefficient)
        {
            if (degree < 0)
            {
                throw new ArgumentException();
            }
            if (coefficient == 0)
            {
                return this.field.Zero;
            }
            int length = this.coefficients.Length;
            int[] coefficients = new int[length + degree];
            for (int i = 0; i < length; i++)
            {
                coefficients[i] = this.field.multiply(this.coefficients[i], coefficient);
            }
            return new GenericGFPoly(this.field, coefficients);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(8 * this.Degree);
            for (int i = this.Degree; i >= 0; i--)
            {
                int a = this.getCoefficient(i);
                if (a == 0)
                {
                    continue;
                }
                if (a < 0)
                {
                    builder.Append(" - ");
                    a = -a;
                }
                else if (builder.Length > 0)
                {
                    builder.Append(" + ");
                }
                if ((i == 0) || (a != 1))
                {
                    int num3 = this.field.log(a);
                    switch (num3)
                    {
                        case 0:
                            builder.Append('1');
                            goto Label_0097;

                        case 1:
                            builder.Append('a');
                            goto Label_0097;
                    }
                    builder.Append("a^");
                    builder.Append(num3);
                }
            Label_0097:
                switch (i)
                {
                    case 0:
                    {
                        continue;
                    }
                    case 1:
                    {
                        builder.Append('x');
                        continue;
                    }
                    default:
                    {
                        builder.Append("x^");
                        builder.Append(i);
                        continue;
                    }
                }
            }
            return builder.ToString();
        }

        internal int[] Coefficients
        {
            get
            {
                return this.coefficients;
            }
        }

        internal int Degree
        {
            get
            {
                return (this.coefficients.Length - 1);
            }
        }

        internal bool isZero
        {
            get
            {
                return (this.coefficients[0] == 0);
            }
        }
    }
}

