namespace NativeSharp.Lib.Resolvers;

public class CppCodeAttribute(string code, string headers, string libs) : Attribute
{
    public CppNativeContent NativeContent { get; } = new(code, [headers], [libs]);
}