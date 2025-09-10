using NativeSharp.Common;
using NativeSharp.Lib.System;

namespace NativeSharp.Lib.Tests;

static class SystemStringUtilities
{
    public static System_String ToSystemString(this string text)
    {
        var decoded = StringEncoders.EncodeBytes(text);
        var result = new System_String()
        {
            Coder = decoded.Coder,
            Data = decoded.Data,
        };
        return result;
    }

    public static string ToClrString(this System_String text)
    {
        string result = text.Data.ToSystemString(text.Coder);
        return result;
    }
}