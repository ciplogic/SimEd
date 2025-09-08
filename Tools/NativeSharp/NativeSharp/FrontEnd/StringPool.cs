namespace NativeSharp.Operations.Values;

class StringPool
{
    public Dictionary<string, int> Pool { get; } = new();

    public List<int> Coders { get; } = [];
    public List<byte[]> Values { get; } = [];

    public static StringPool Instance { get; } = new();

    public int GetIndex(string value)
    {
        if (Pool.TryGetValue(value, out int result))
        {
            return result;
        }

        result = Pool.Count;
        (byte[] Data, int Coder) data = EncodeBytes(value);
        Pool.Add(value, result);
        Coders.Add(data.Coder);
        Values.Add(data.Data);
        return result;
    }

    private static (byte[] Data, int Coder) EncodeBytes(string value)
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
}