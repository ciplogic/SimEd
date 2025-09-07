using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class LoadElementOp : BaseOp
{
    public VReg ResultElement { get; }
    public IndexedVariable Array { get; }
    public IValueExpression Index { get; }

    public LoadElementOp(VReg resultElement, IndexedVariable array, IValueExpression index)
    {
        ResultElement = resultElement;
        Array = array;
        Index = index;
    }

    public override string GenCode() 
        => $"{ResultElement.GenCode()} = (*{Array.GenCode()})[{Index.Code()}];";
}