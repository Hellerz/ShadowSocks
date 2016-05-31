﻿namespace ZXing.Common
{
    using System;

    public sealed class DefaultGridSampler : GridSampler
    {
        public override BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform)
        {
            if ((dimensionX <= 0) || (dimensionY <= 0))
            {
                return null;
            }
            BitMatrix matrix = new BitMatrix(dimensionX, dimensionY);
            float[] points = new float[dimensionX << 1];
            for (int i = 0; i < dimensionY; i++)
            {
                int length = points.Length;
                float num3 = i + 0.5f;
                for (int j = 0; j < length; j += 2)
                {
                    points[j] = (j >> 1) + 0.5f;
                    points[j + 1] = num3;
                }
                transform.transformPoints(points);
                if (!GridSampler.checkAndNudgePoints(image, points))
                {
                    return null;
                }
                try
                {
                    for (int k = 0; k < length; k += 2)
                    {
                        matrix[k >> 1, i] = image[(int) points[k], (int) points[k + 1]];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    return null;
                }
            }
            return matrix;
        }

        public override BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY)
        {
            PerspectiveTransform transform = PerspectiveTransform.quadrilateralToQuadrilateral(p1ToX, p1ToY, p2ToX, p2ToY, p3ToX, p3ToY, p4ToX, p4ToY, p1FromX, p1FromY, p2FromX, p2FromY, p3FromX, p3FromY, p4FromX, p4FromY);
            return this.sampleGrid(image, dimensionX, dimensionY, transform);
        }
    }
}

