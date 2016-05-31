namespace ZXing
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public class BitmapLuminanceSource : BaseLuminanceSource
    {
        public BitmapLuminanceSource(Bitmap bitmap) : base(bitmap.Width, bitmap.Height)
        {
            int height = bitmap.Height;
            int width = bitmap.Width;
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                int length = Math.Abs(bitmapdata.Stride);
                int num4 = length / width;
                if (num4 > 4)
                {
                    for (int i = 0; i < height; i++)
                    {
                        int num6 = i * width;
                        for (int j = 0; j < width; j++)
                        {
                            Color pixel = bitmap.GetPixel(j, i);
                            base.luminances[num6 + j] = (byte) ((((0x4c6a * pixel.R) + (0x9696 * pixel.G)) + (0x1d00 * pixel.B)) >> 0x10);
                        }
                    }
                }
                else
                {
                    int stride = bitmapdata.Stride;
                    byte[] destination = new byte[length];
                    IntPtr source = bitmapdata.Scan0;
                    byte[] buffer2 = new byte[bitmap.Palette.Entries.Length];
                    for (int k = 0; k < bitmap.Palette.Entries.Length; k++)
                    {
                        Color color2 = bitmap.Palette.Entries[k];
                        buffer2[k] = (byte) ((((0x4c6a * color2.R) + (0x9696 * color2.G)) + (0x1d00 * color2.B)) >> 0x10);
                    }
                    if ((bitmap.PixelFormat == PixelFormat.Format32bppArgb) || (bitmap.PixelFormat == PixelFormat.Format32bppPArgb))
                    {
                        num4 = 40;
                    }
                    if ((bitmap.PixelFormat == ((PixelFormat) 0x200f)) || ((bitmap.Flags & 0x20) == 0x20))
                    {
                        num4 = 0x29;
                    }
                    for (int m = 0; m < height; m++)
                    {
                        int num12;
                        int num13;
                        int num15;
                        int num16;
                        int num17;
                        byte num18;
                        int num26;
                        int num27;
                        byte num28;
                        int num29;
                        int num30;
                        byte num31;
                        int num32;
                        int num33;
                        byte num34;
                        int num36;
                        int num37;
                        byte num38;
                        Marshal.Copy(source, destination, 0, length);
                        source = new IntPtr(source.ToInt64() + stride);
                        int index = m * width;
                        switch (num4)
                        {
                            case 0:
                                num12 = 0;
                                goto Label_0239;

                            case 1:
                                num15 = 0;
                                goto Label_0264;

                            case 2:
                                num16 = 2 * width;
                                num17 = 0;
                                goto Label_0311;

                            case 3:
                                num26 = width * 3;
                                num27 = 0;
                                goto Label_036D;

                            case 4:
                                num29 = 4 * width;
                                num30 = 0;
                                goto Label_03C6;

                            case 40:
                                num32 = 4 * width;
                                num33 = 0;
                                goto Label_0445;

                            case 0x29:
                                num36 = 4 * width;
                                num37 = 0;
                                goto Label_04A1;

                            default:
                                throw new NotSupportedException();
                        }
                    Label_01F2:
                        num13 = 0;
                        while ((num13 < 8) && (((8 * num12) + num13) < width))
                        {
                            int num14 = (destination[num12] >> (7 - num13)) & 1;
                            base.luminances[(index + (8 * num12)) + num13] = buffer2[num14];
                            num13++;
                        }
                        num12++;
                    Label_0239:
                        if ((num12 * 8) < width)
                        {
                            goto Label_01F2;
                        }
                        goto Label_04AF;
                    Label_024A:
                        base.luminances[index + num15] = buffer2[destination[num15]];
                        num15++;
                    Label_0264:
                        if (num15 < width)
                        {
                            goto Label_024A;
                        }
                        goto Label_04AF;
                    Label_027B:
                        num18 = destination[num17];
                        byte num19 = destination[num17 + 1];
                        int num20 = num18 & 0x1f;
                        int num21 = (((num18 & 0xe0) >> 5) | ((num19 & 3) << 3)) & 0x1f;
                        int num22 = (num19 >> 2) & 0x1f;
                        int num23 = ((num22 * 0x20f) + 0x17) >> 6;
                        int num24 = ((num21 * 0x20f) + 0x17) >> 6;
                        int num25 = ((num20 * 0x20f) + 0x17) >> 6;
                        base.luminances[index] = (byte) ((((0x4c6a * num23) + (0x9696 * num24)) + (0x1d00 * num25)) >> 0x10);
                        index++;
                        num17 += 2;
                    Label_0311:
                        if (num17 < num16)
                        {
                            goto Label_027B;
                        }
                        goto Label_04AF;
                    Label_0329:
                        num28 = (byte) ((((0x1d00 * destination[num27]) + (0x9696 * destination[num27 + 1])) + (0x4c6a * destination[num27 + 2])) >> 0x10);
                        base.luminances[index] = num28;
                        index++;
                        num27 += 3;
                    Label_036D:
                        if (num27 < num26)
                        {
                            goto Label_0329;
                        }
                        goto Label_04AF;
                    Label_0382:
                        num31 = (byte) ((((0x1d00 * destination[num30]) + (0x9696 * destination[num30 + 1])) + (0x4c6a * destination[num30 + 2])) >> 0x10);
                        base.luminances[index] = num31;
                        index++;
                        num30 += 4;
                    Label_03C6:
                        if (num30 < num29)
                        {
                            goto Label_0382;
                        }
                        goto Label_04AF;
                    Label_03DB:
                        num34 = (byte) ((((0x1d00 * destination[num33]) + (0x9696 * destination[num33 + 1])) + (0x4c6a * destination[num33 + 2])) >> 0x10);
                        byte num35 = destination[num33 + 3];
                        base.luminances[index] = (byte) ((((num34 * num35) >> 8) + ((0xff * (0xff - num35)) >> 8)) + 1);
                        index++;
                        num33 += 4;
                    Label_0445:
                        if (num33 < num32)
                        {
                            goto Label_03DB;
                        }
                        goto Label_04AF;
                    Label_0457:
                        num38 = (byte) (0xff - ((((0x1d00 * destination[num37]) + (0x9696 * destination[num37 + 1])) + (0x4c6a * destination[num37 + 2])) >> 0x10));
                        base.luminances[index] = num38;
                        index++;
                        num37 += 4;
                    Label_04A1:
                        if (num37 < num36)
                        {
                            goto Label_0457;
                        }
                    Label_04AF:;
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapdata);
            }
        }

        protected BitmapLuminanceSource(int width, int height) : base(width, height)
        {
        }

        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
        {
            BitmapLuminanceSource source = new BitmapLuminanceSource(width, height);
            source.luminances = newLuminances;
            return source;
        }
    }
}

