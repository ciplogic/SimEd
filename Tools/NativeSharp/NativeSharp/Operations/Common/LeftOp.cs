using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.Common;

internal abstract class LeftOp(IndexedVariable left) : BaseOp
{
    public IndexedVariable Left { get; } = left;
}