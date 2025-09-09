using NativeSharp.Lib.Resolvers;

namespace NativeSharp.Lib;

public static class ResolvedMethods
{
    public static string System_Boolean_ToString(bool text)
    {
        return text ? "true" : "false";
    }

    public static System_String System_String_Concat(System_String text, System_String text2)
    {
        var data = new byte[text.Length + text2.Length];
        Array.Copy(text.Data, data, text.Length);

        Array.Copy(text.Data, 0, data, text.Length, text2.Length);
        System_String result = new()
        {
            Coder = 0,
            Data = data,
        };
        return result;
    }

    [CppCode("wsprintf(\"%s\", arg_0->Data);", "stdio.h", "")]
    public static void System_Console_WriteLine(string text)
    {
        //Nothing for now. Will be filled by C++ code
    }

    public static int System_String_get_Length(System_String text) => text.Data.Length;

    public static void System_Array_Copy(byte[] sourceArray, byte[] destinationArray, int len)
    {
        for (var i = 0; i < len; i++)
        {
            destinationArray[i] = sourceArray[i];
        }
    }

    public static void System_Array_Copy(byte[] sourceArray, int sourceIndex, byte[] destinationArray,
        int destinationIndex, int len)
    {
        for (var i = 0; i < len; i++)
        {
            int index = sourceIndex + i;
            int destArrayIndex = destinationIndex + i;
            byte byteValue = sourceArray[index];
            destinationArray[destArrayIndex] = byteValue;
        }
    }
}