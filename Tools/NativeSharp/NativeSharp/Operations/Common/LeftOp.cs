using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.Common;

public abstract class LeftOp(IndexedVariable left) : BaseOp
{
    public IndexedVariable Left { get; set; } = left;
}