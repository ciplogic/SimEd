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
        System_String result = new System_String()
        {
            Coder = 0,
            Data = data,
        };
        return result;
    }

    public static void System_Console_WriteLine(string text)
    {
        //Nothing for now. Will be filled by C++ code
    }
    public static int System_String_get_Length(System_String text) => text.Length;
}