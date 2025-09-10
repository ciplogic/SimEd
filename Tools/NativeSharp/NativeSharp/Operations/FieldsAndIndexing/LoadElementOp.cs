using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class LoadElementOp : LeftOp
{
    public IndexedVariable Array { get; }
    public IValueExpression Index { get; }

    public LoadElementOp(VReg resultElement, IndexedVariable array, IValueExpression index)
        : base(resultElement)
    {
        Array = array;
        Index = index;
    }

    public override string GenCode()
        => $"{Left.GenCode()} = (*{Array.GenCode()})[{Index.Code()}];";
}