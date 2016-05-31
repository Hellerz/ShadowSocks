namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ZXing.Common;

    internal static class DecodedBitStreamParser
    {
        private static readonly char[] ALPHANUMERIC_CHARS = new char[] { 
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 
            'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 
            'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':'
         };
        private const int GB2312_SUBSET = 1;

        internal static DecoderResult decode(byte[] bytes, Version version, ErrorCorrectionLevel ecLevel, IDictionary<DecodeHintType, object> hints)
        {
            BitSource bits = new BitSource(bytes);
            StringBuilder result = new StringBuilder(50);
            List<byte[]> byteSegments = new List<byte[]>(1);
            int saSequence = -1;
            int saParity = -1;
            try
            {
                Mode tERMINATOR;
                bool flag = false;
                do
                {
                    if (bits.available() < 4)
                    {
                        tERMINATOR = Mode.TERMINATOR;
                    }
                    else
                    {
                        try
                        {
                            tERMINATOR = Mode.forBits(bits.readBits(4));
                        }
                        catch (ArgumentException)
                        {
                            return null;
                        }
                    }
                    if (tERMINATOR != Mode.TERMINATOR)
                    {
                        if ((tERMINATOR == Mode.FNC1_FIRST_POSITION) || (tERMINATOR == Mode.FNC1_SECOND_POSITION))
                        {
                            flag = true;
                        }
                        else if (tERMINATOR == Mode.STRUCTURED_APPEND)
                        {
                            if (bits.available() < 0x10)
                            {
                                return null;
                            }
                            saSequence = bits.readBits(8);
                            saParity = bits.readBits(8);
                        }
                        else if (tERMINATOR != Mode.ECI)
                        {
                            if (tERMINATOR == Mode.HANZI)
                            {
                                int num3 = bits.readBits(4);
                                int count = bits.readBits(tERMINATOR.getCharacterCountBits(version));
                                if ((num3 == 1) && !decodeHanziSegment(bits, result, count))
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                int num5 = bits.readBits(tERMINATOR.getCharacterCountBits(version));
                                if (tERMINATOR == Mode.NUMERIC)
                                {
                                    if (!decodeNumericSegment(bits, result, num5))
                                    {
                                        return null;
                                    }
                                }
                                else if (tERMINATOR == Mode.ALPHANUMERIC)
                                {
                                    if (!decodeAlphanumericSegment(bits, result, num5, flag))
                                    {
                                        return null;
                                    }
                                }
                                else if (tERMINATOR == Mode.BYTE)
                                {
                                    if (!decodeByteSegment(bits, result, num5, byteSegments, hints))
                                    {
                                        return null;
                                    }
                                }
                                else if (tERMINATOR == Mode.KANJI)
                                {
                                    if (!decodeKanjiSegment(bits, result, num5))
                                    {
                                        return null;
                                    }
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }
                while (tERMINATOR != Mode.TERMINATOR);
            }
            catch (ArgumentException)
            {
                return null;
            }
            return new DecoderResult(bytes, result.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine), (byteSegments.Count == 0) ? null : ((IList<byte[]>) byteSegments), (ecLevel == null) ? null : ecLevel.ToString(), saSequence, saParity);
        }

        private static bool decodeAlphanumericSegment(BitSource bits, StringBuilder result, int count, bool fc1InEffect)
        {
            int length = result.Length;
            while (count > 1)
            {
                if (bits.available() < 11)
                {
                    return false;
                }
                int num2 = bits.readBits(11);
                result.Append(toAlphaNumericChar(num2 / 0x2d));
                result.Append(toAlphaNumericChar(num2 % 0x2d));
                count -= 2;
            }
            if (count == 1)
            {
                if (bits.available() < 6)
                {
                    return false;
                }
                result.Append(toAlphaNumericChar(bits.readBits(6)));
            }
            if (fc1InEffect)
            {
                for (int i = length; i < result.Length; i++)
                {
                    if (result[i] == '%')
                    {
                        if ((i < (result.Length - 1)) && (result[i + 1] == '%'))
                        {
                            result.Remove(i + 1, 1);
                        }
                        else
                        {
                            result.Remove(i, 1);
                            result.Insert(i, new char[] { '\x001d' });
                        }
                    }
                }
            }
            return true;
        }

        private static bool decodeByteSegment(BitSource bits, StringBuilder result, int count, IList<byte[]> byteSegments, IDictionary<DecodeHintType, object> hints)
        {
            if ((count << 3) > bits.available())
            {
                return false;
            }
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = (byte) bits.readBits(8);
            }
            string name = StringUtils.guessEncoding(bytes, hints);
            try
            {
                result.Append(Encoding.GetEncoding(name).GetString(bytes, 0, bytes.Length));
            }
            catch (Exception)
            {
                return false;
            }
            byteSegments.Add(bytes);
            return true;
        }

        private static bool decodeHanziSegment(BitSource bits, StringBuilder result, int count)
        {
            if ((count * 13) > bits.available())
            {
                return false;
            }
            byte[] bytes = new byte[2 * count];
            int index = 0;
            while (count > 0)
            {
                int num2 = bits.readBits(13);
                int num3 = ((num2 / 0x60) << 8) | (num2 % 0x60);
                if (num3 < 0x3bf)
                {
                    num3 += 0xa1a1;
                }
                else
                {
                    num3 += 0xa6a1;
                }
                bytes[index] = (byte) ((num3 >> 8) & 0xff);
                bytes[index + 1] = (byte) (num3 & 0xff);
                index += 2;
                count--;
            }
            try
            {
                result.Append(Encoding.GetEncoding(StringUtils.GB2312).GetString(bytes, 0, bytes.Length));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static bool decodeKanjiSegment(BitSource bits, StringBuilder result, int count)
        {
            if ((count * 13) > bits.available())
            {
                return false;
            }
            byte[] bytes = new byte[2 * count];
            int index = 0;
            while (count > 0)
            {
                int num2 = bits.readBits(13);
                int num3 = ((num2 / 0xc0) << 8) | (num2 % 0xc0);
                if (num3 < 0x1f00)
                {
                    num3 += 0x8140;
                }
                else
                {
                    num3 += 0xc140;
                }
                bytes[index] = (byte) (num3 >> 8);
                bytes[index + 1] = (byte) num3;
                index += 2;
                count--;
            }
            try
            {
                result.Append(Encoding.GetEncoding(StringUtils.SHIFT_JIS).GetString(bytes, 0, bytes.Length));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static bool decodeNumericSegment(BitSource bits, StringBuilder result, int count)
        {
            while (count >= 3)
            {
                if (bits.available() < 10)
                {
                    return false;
                }
                int num = bits.readBits(10);
                if (num >= 0x3e8)
                {
                    return false;
                }
                result.Append(toAlphaNumericChar(num / 100));
                result.Append(toAlphaNumericChar((num / 10) % 10));
                result.Append(toAlphaNumericChar(num % 10));
                count -= 3;
            }
            if (count == 2)
            {
                if (bits.available() < 7)
                {
                    return false;
                }
                int num2 = bits.readBits(7);
                if (num2 >= 100)
                {
                    return false;
                }
                result.Append(toAlphaNumericChar(num2 / 10));
                result.Append(toAlphaNumericChar(num2 % 10));
            }
            else if (count == 1)
            {
                if (bits.available() < 4)
                {
                    return false;
                }
                int num3 = bits.readBits(4);
                if (num3 >= 10)
                {
                    return false;
                }
                result.Append(toAlphaNumericChar(num3));
            }
            return true;
        }

        private static int parseECIValue(BitSource bits)
        {
            int num = bits.readBits(8);
            if ((num & 0x80) == 0)
            {
                return (num & 0x7f);
            }
            if ((num & 0xc0) == 0x80)
            {
                int num2 = bits.readBits(8);
                return (((num & 0x3f) << 8) | num2);
            }
            if ((num & 0xe0) != 0xc0)
            {
                throw new ArgumentException("Bad ECI bits starting with byte " + num);
            }
            int num3 = bits.readBits(0x10);
            return (((num & 0x1f) << 0x10) | num3);
        }

        private static char toAlphaNumericChar(int value)
        {
            int length = ALPHANUMERIC_CHARS.Length;
            return ALPHANUMERIC_CHARS[value];
        }
    }
}

