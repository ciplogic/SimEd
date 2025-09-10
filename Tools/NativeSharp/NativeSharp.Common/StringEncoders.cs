namespace NativeSharp.Common;

public static class StringEncoders
{
    public static TOut[] SelectToArray<TIn, TOut>(this TIn[] source, Func<TIn, TOut> selector)
    {
        TOut[] result = new TOut[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }

        return result;
    }

    public static (byte[] Data, int Coder) EncodeBytes(string value)
    {
        bool areAllLatin = string.IsNullOrEmpty(value) || value.All(x => x <= 255);
        return areAllLatin
            ? (DecodeLatin(value), 0)
            : (DecodeUnicode(value), 1);
    }

    private static byte[] DecodeUnicode(string value)
    {
        byte[] result = new byte[value.Length * 2];

        for (int index = 0; index < value.Length; index++)
        {
            char ch = value[index];
            result[index] = (byte)ch;
            result[index + 1] = (byte)(ch >> 8);
        }

        return result;
    }

    private static byte[] DecodeLatin(string value)
    {
        byte[] result = new byte[value.Length];

        for (int index = 0; index < value.Length; index++)
        {
            char ch = value[index];
            result[index] = (byte)ch;
        }

        return result;
    }

    public static string ToSystemString(this byte[] bytes, int coder)
    {
        if (coder == 0)
        {
            var chars = bytes.SelectToArray(b => (char)b);
            return new string(chars);
        }

        var charsU16 = new char[bytes.Length / 2];
        for (int index = 0; index < bytes.Length; index += 2)
        {
            char ch = (char)(bytes[index] + (bytes[index + 1] << 8));
            charsU16[index / 2] = (char)ch;
        }

        return new string(charsU16);
    }
}