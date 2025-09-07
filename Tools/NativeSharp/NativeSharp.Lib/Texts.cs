using NativeSharp.Lib;

public static class Texts
{
    public static System_String FromIndex(int index, int[] codes, int[] endPositions, byte[] data)
    {
        var startPos = 0;
        if (index > 0)
        {
            startPos = endPositions[index - 1];
        }
        var endPos = endPositions[index];
        var len =  endPos - startPos;
        return BuildSystemString(index, codes, data, len, startPos);
    }

    public static System_String BuildSystemString(int index, int[] codes, byte[] data, int len, int startPos)
    {
        var resultData = new byte[len];
        Array.Copy(data, startPos, resultData, 0, len);
        System_String result = new System_String();
        result.Coder = codes[index];
        result.Data = resultData;
        return result;
    }
}