using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class LdLenOp : LeftOp
{
    public IndexedVariable Right { get; }

    public LdLenOp(IndexedVariable left, IndexedVariable right)
        : base(left)
    {
        Right = right;
    }

    public override string GenCode()
        => $"{Left.GenCode()} = {Right.GenCode()}->size();";
}