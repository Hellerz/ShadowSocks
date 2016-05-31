namespace ZXing.Common.ReedSolomon
{
    using System;

    public sealed class ReedSolomonDecoder
    {
        private readonly GenericGF field;

        public ReedSolomonDecoder(GenericGF field)
        {
            this.field = field;
        }

        public bool decode(int[] received, int twoS)
        {
            GenericGFPoly poly = new GenericGFPoly(this.field, received);
            int[] coefficients = new int[twoS];
            bool flag = true;
            for (int i = 0; i < twoS; i++)
            {
                int num2 = poly.evaluateAt(this.field.exp(i + this.field.GeneratorBase));
                coefficients[(coefficients.Length - 1) - i] = num2;
                if (num2 != 0)
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                GenericGFPoly b = new GenericGFPoly(this.field, coefficients);
                GenericGFPoly[] polyArray = this.runEuclideanAlgorithm(this.field.buildMonomial(twoS, 1), b, twoS);
                if (polyArray == null)
                {
                    return false;
                }
                GenericGFPoly errorLocator = polyArray[0];
                int[] errorLocations = this.findErrorLocations(errorLocator);
                if (errorLocations == null)
                {
                    return false;
                }
                GenericGFPoly errorEvaluator = polyArray[1];
                int[] numArray3 = this.findErrorMagnitudes(errorEvaluator, errorLocations);
                for (int j = 0; j < errorLocations.Length; j++)
                {
                    int index = (received.Length - 1) - this.field.log(errorLocations[j]);
                    if (index < 0)
                    {
                        return false;
                    }
                    received[index] = GenericGF.addOrSubtract(received[index], numArray3[j]);
                }
            }
            return true;
        }

        private int[] findErrorLocations(GenericGFPoly errorLocator)
        {
            int degree = errorLocator.Degree;
            if (degree == 1)
            {
                return new int[] { errorLocator.getCoefficient(1) };
            }
            int[] numArray = new int[degree];
            int index = 0;
            for (int i = 1; (i < this.field.Size) && (index < degree); i++)
            {
                if (errorLocator.evaluateAt(i) == 0)
                {
                    numArray[index] = this.field.inverse(i);
                    index++;
                }
            }
            if (index != degree)
            {
                return null;
            }
            return numArray;
        }

        private int[] findErrorMagnitudes(GenericGFPoly errorEvaluator, int[] errorLocations)
        {
            int length = errorLocations.Length;
            int[] numArray = new int[length];
            for (int i = 0; i < length; i++)
            {
                int b = this.field.inverse(errorLocations[i]);
                int a = 1;
                for (int j = 0; j < length; j++)
                {
                    if (i != j)
                    {
                        int num6 = this.field.multiply(errorLocations[j], b);
                        int num7 = ((num6 & 1) == 0) ? (num6 | 1) : (num6 & -2);
                        a = this.field.multiply(a, num7);
                    }
                }
                numArray[i] = this.field.multiply(errorEvaluator.evaluateAt(b), this.field.inverse(a));
                if (this.field.GeneratorBase != 0)
                {
                    numArray[i] = this.field.multiply(numArray[i], b);
                }
            }
            return numArray;
        }

        internal GenericGFPoly[] runEuclideanAlgorithm(GenericGFPoly a, GenericGFPoly b, int R)
        {
            if (a.Degree < b.Degree)
            {
                GenericGFPoly poly = a;
                a = b;
                b = poly;
            }
            GenericGFPoly poly2 = a;
            GenericGFPoly poly3 = b;
            GenericGFPoly zero = this.field.Zero;
            GenericGFPoly one = this.field.One;
            while (poly3.Degree >= (R / 2))
            {
                GenericGFPoly poly6 = poly2;
                GenericGFPoly other = zero;
                poly2 = poly3;
                zero = one;
                if (poly2.isZero)
                {
                    return null;
                }
                poly3 = poly6;
                GenericGFPoly poly8 = this.field.Zero;
                int num = poly2.getCoefficient(poly2.Degree);
                int num2 = this.field.inverse(num);
                while ((poly3.Degree >= poly2.Degree) && !poly3.isZero)
                {
                    int degree = poly3.Degree - poly2.Degree;
                    int coefficient = this.field.multiply(poly3.getCoefficient(poly3.Degree), num2);
                    poly8 = poly8.addOrSubtract(this.field.buildMonomial(degree, coefficient));
                    poly3 = poly3.addOrSubtract(poly2.multiplyByMonomial(degree, coefficient));
                }
                one = poly8.multiply(zero).addOrSubtract(other);
                if (poly3.Degree >= poly2.Degree)
                {
                    return null;
                }
            }
            int num5 = one.getCoefficient(0);
            if (num5 == 0)
            {
                return null;
            }
            int scalar = this.field.inverse(num5);
            GenericGFPoly poly9 = one.multiply(scalar);
            GenericGFPoly poly10 = poly3.multiply(scalar);
            return new GenericGFPoly[] { poly9, poly10 };
        }
    }
}

