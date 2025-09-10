using NativeSharp.Common;

namespace NativeSharp.FrontEnd;

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
        (byte[] Data, int Coder) data = StringEncoders.EncodeBytes(value);
        Pool.Add(value, result);
        Coders.Add(data.Coder);
        Values.Add(data.Data);
        return result;
    }
}