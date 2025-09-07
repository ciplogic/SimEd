using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.Common;

public class CilNativeMethod : BaseNativeMethod
{
    public BaseOp[] Instructions { get; set; } = [];
    public IndexedVariable[] Locals { get; set; }
}