using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.Common;

public class BaseNativeMethod
{
    public ArgumentVariable[] Args { get; set; }
    public MethodBase Target { get; set; } = null!;

    public override string ToString() => Target.MangleMethodName();
}