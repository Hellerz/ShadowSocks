namespace ZXing.Common
{
    using System;
    using System.Collections.Generic;
    using ZXing;

    public static class StringUtils
    {
        private static readonly bool ASSUME_SHIFT_JIS = ((string.Compare(SHIFT_JIS, PLATFORM_DEFAULT_ENCODING, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare("EUC-JP", PLATFORM_DEFAULT_ENCODING, StringComparison.OrdinalIgnoreCase) == 0));
        private const string EUC_JP = "EUC-JP";
        public static string GB2312 = "GB2312";
        private const string ISO88591 = "ISO-8859-1";
        private static string PLATFORM_DEFAULT_ENCODING = Encoding.Default.WebName;
        public static string SHIFT_JIS = "SJIS";
        private const string UTF8 = "UTF-8";

        public static string guessEncoding(byte[] bytes, IDictionary<DecodeHintType, object> hints)
        {
            if ((hints != null) && hints.ContainsKey(DecodeHintType.CHARACTER_SET))
            {
                string str = (string) hints[DecodeHintType.CHARACTER_SET];
                if (str != null)
                {
                    return str;
                }
            }
            int length = bytes.Length;
            bool flag = true;
            bool flag2 = true;
            bool flag3 = true;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            int num11 = 0;
            int num12 = 0;
            bool flag4 = (((bytes.Length > 3) && (bytes[0] == 0xef)) && (bytes[1] == 0xbb)) && (bytes[2] == 0xbf);
            for (int i = 0; (i < length) && ((flag || flag2) || flag3); i++)
            {
                int num14 = bytes[i] & 0xff;
                if (flag3)
                {
                    if (num2 > 0)
                    {
                        if ((num14 & 0x80) == 0)
                        {
                            flag3 = false;
                        }
                        else
                        {
                            num2--;
                        }
                    }
                    else if ((num14 & 0x80) != 0)
                    {
                        if ((num14 & 0x40) == 0)
                        {
                            flag3 = false;
                        }
                        else
                        {
                            num2++;
                            if ((num14 & 0x20) == 0)
                            {
                                num3++;
                            }
                            else
                            {
                                num2++;
                                if ((num14 & 0x10) == 0)
                                {
                                    num4++;
                                }
                                else
                                {
                                    num2++;
                                    if ((num14 & 8) == 0)
                                    {
                                        num5++;
                                    }
                                    else
                                    {
                                        flag3 = false;
                                    }
                                }
                            }
                        }
                    }
                }
                if (flag)
                {
                    if ((num14 > 0x7f) && (num14 < 160))
                    {
                        flag = false;
                    }
                    else if ((num14 > 0x9f) && (((num14 < 0xc0) || (num14 == 0xd7)) || (num14 == 0xf7)))
                    {
                        num12++;
                    }
                }
                if (flag2)
                {
                    if (num6 > 0)
                    {
                        if (((num14 < 0x40) || (num14 == 0x7f)) || (num14 > 0xfc))
                        {
                            flag2 = false;
                        }
                        else
                        {
                            num6--;
                        }
                    }
                    else if (((num14 == 0x80) || (num14 == 160)) || (num14 > 0xef))
                    {
                        flag2 = false;
                    }
                    else if ((num14 > 160) && (num14 < 0xe0))
                    {
                        num7++;
                        num9 = 0;
                        num8++;
                        if (num8 > num10)
                        {
                            num10 = num8;
                        }
                    }
                    else if (num14 > 0x7f)
                    {
                        num6++;
                        num8 = 0;
                        num9++;
                        if (num9 > num11)
                        {
                            num11 = num9;
                        }
                    }
                    else
                    {
                        num8 = 0;
                        num9 = 0;
                    }
                }
            }
            if (flag3 && (num2 > 0))
            {
                flag3 = false;
            }
            if (flag2 && (num6 > 0))
            {
                flag2 = false;
            }
            if (flag3 && (flag4 || (((num3 + num4) + num5) > 0)))
            {
                return "UTF-8";
            }
            if (flag2 && ((ASSUME_SHIFT_JIS || (num10 >= 3)) || (num11 >= 3)))
            {
                return SHIFT_JIS;
            }
            if (flag && flag2)
            {
                if (((num10 != 2) || (num7 != 2)) && ((num12 * 10) < length))
                {
                    return "ISO-8859-1";
                }
                return SHIFT_JIS;
            }
            if (flag)
            {
                return "ISO-8859-1";
            }
            if (flag2)
            {
                return SHIFT_JIS;
            }
            if (flag3)
            {
                return "UTF-8";
            }
            return PLATFORM_DEFAULT_ENCODING;
        }
    }
}

