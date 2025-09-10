using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

internal class StoreElementOp(IndexedVariable arrPtr, IValueExpression index, IValueExpression valueToSet) : BaseOp
{
    public IndexedVariable ArrPtr { get; } = arrPtr;
    public IValueExpression Index { get; } = index;
    public IValueExpression ValueToSet { get; } = valueToSet;

    public override string GenCode()
    {
        return $"(*{ArrPtr.Code()})[{Index.Code()}] = {ValueToSet.Code()};";
    }
}