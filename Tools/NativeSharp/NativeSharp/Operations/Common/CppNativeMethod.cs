using NativeSharp.Lib;

namespace NativeSharp.Operations.Common;

public class CppNativeMethod(CppNativeContent content) : BaseNativeMethod
{
    public CppNativeContent Content { get; } = content; 
}