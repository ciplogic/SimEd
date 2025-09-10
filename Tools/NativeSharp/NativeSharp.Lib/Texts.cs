using NativeSharp.Lib;
using NativeSharp.Lib.System;

public static class Texts
{
    public static System_String FromIndex(int index, int[] codes, int[] startPos, int[] lengths, byte[] data)
    {
        var start = startPos[index];
        var len = lengths[index];
        var code = codes[index];
        return BuildSystemString(code, data, start, len);
    }

    public static System_String BuildSystemString(int code, byte[] data, int startPos, int len)
    {
        var resultData = new byte[len];
        Array.Copy(data, startPos, resultData, 0, len);
        System_String result = new()
        {
            Coder = code,
            Data = resultData
        };
        return result;
    }
}