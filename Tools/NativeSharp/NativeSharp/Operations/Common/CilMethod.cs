using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.Common;

public class CilMethod : BaseMethod
{
    public BaseOp[] Instructions { get; set; } = [];
    public IndexedVariable[] Locals { get; set; }
    public ArgumentVariable[] Args { get; set; }
}