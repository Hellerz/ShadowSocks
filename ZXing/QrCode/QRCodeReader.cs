namespace ZXing.QrCode
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using ZXing;
    using ZXing.Common;
    using ZXing.QrCode.Internal;

    public class QRCodeReader
    {
        private readonly Decoder decoder = new Decoder();
        private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

        public Result decode(BinaryBitmap image)
        {
            return this.decode(image, null);
        }

        public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
        {
            DecoderResult result;
            ResultPoint[] points;
            if ((image == null) || (image.BlackMatrix == null))
            {
                return null;
            }
            if ((hints != null) && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
            {
                BitMatrix bits = extractPureBits(image.BlackMatrix);
                if (bits == null)
                {
                    return null;
                }
                result = this.decoder.decode(bits, hints);
                points = NO_POINTS;
            }
            else
            {
                DetectorResult result2 = new Detector(image.BlackMatrix).detect(hints);
                if (result2 == null)
                {
                    return null;
                }
                result = this.decoder.decode(result2.Bits, hints);
                points = result2.Points;
            }
            if (result == null)
            {
                return null;
            }
            QRCodeDecoderMetaData other = result.Other as QRCodeDecoderMetaData;
            if (other != null)
            {
                other.applyMirroredCorrection(points);
            }
            Result result3 = new Result(result.Text, result.RawBytes, points, BarcodeFormat.QR_CODE);
            IList<byte[]> byteSegments = result.ByteSegments;
            if (byteSegments != null)
            {
                result3.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
            }
            string eCLevel = result.ECLevel;
            if (eCLevel != null)
            {
                result3.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, eCLevel);
            }
            if (result.StructuredAppend)
            {
                result3.putMetadata(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE, result.StructuredAppendSequenceNumber);
                result3.putMetadata(ResultMetadataType.STRUCTURED_APPEND_PARITY, result.StructuredAppendParity);
            }
            return result3;
        }

        private static BitMatrix extractPureBits(BitMatrix image)
        {
            float num;
            int[] leftTopBlack = image.getTopLeftOnBit();
            int[] numArray2 = image.getBottomRightOnBit();
            if ((leftTopBlack == null) || (numArray2 == null))
            {
                return null;
            }
            if (!moduleSize(leftTopBlack, image, out num))
            {
                return null;
            }
            int num2 = leftTopBlack[1];
            int num3 = numArray2[1];
            int num4 = leftTopBlack[0];
            int num5 = numArray2[0];
            if ((num4 >= num5) || (num2 >= num3))
            {
                return null;
            }
            if ((num3 - num2) != (num5 - num4))
            {
                num5 = num4 + (num3 - num2);
            }
            int width = (int) Math.Round((double) (((float) ((num5 - num4) + 1)) / num));
            int height = (int) Math.Round((double) (((float) ((num3 - num2) + 1)) / num));
            if ((width <= 0) || (height <= 0))
            {
                return null;
            }
            if (height != width)
            {
                return null;
            }
            int num8 = (int) (num / 2f);
            num2 += num8;
            num4 += num8;
            int num9 = (num4 + ((int) ((width - 1) * num))) - (num5 - 1);
            if (num9 > 0)
            {
                if (num9 > num8)
                {
                    return null;
                }
                num4 -= num9;
            }
            int num10 = (num2 + ((int) ((height - 1) * num))) - (num3 - 1);
            if (num10 > 0)
            {
                if (num10 > num8)
                {
                    return null;
                }
                num2 -= num10;
            }
            BitMatrix matrix = new BitMatrix(width, height);
            for (int i = 0; i < height; i++)
            {
                int num12 = num2 + ((int) (i * num));
                for (int j = 0; j < width; j++)
                {
                    if (image[num4 + ((int) (j * num)), num12])
                    {
                        matrix[j, i] = true;
                    }
                }
            }
            return matrix;
        }

        protected Decoder getDecoder()
        {
            return this.decoder;
        }

        private static bool moduleSize(int[] leftTopBlack, BitMatrix image, out float msize)
        {
            int height = image.Height;
            int width = image.Width;
            int num3 = leftTopBlack[0];
            int num4 = leftTopBlack[1];
            bool flag = true;
            int num5 = 0;
            while ((num3 < width) && (num4 < height))
            {
                if (flag != image[num3, num4])
                {
                    if (++num5 == 5)
                    {
                        break;
                    }
                    flag = !flag;
                }
                num3++;
                num4++;
            }
            if ((num3 == width) || (num4 == height))
            {
                msize = 0f;
                return false;
            }
            msize = ((float) (num3 - leftTopBlack[0])) / 7f;
            return true;
        }

        public void reset()
        {
        }
    }
}

