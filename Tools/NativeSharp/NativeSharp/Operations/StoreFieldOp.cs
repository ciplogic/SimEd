using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

internal class StoreFieldOp : BaseOp
{
    public IValueExpression ThisPtr { get; }
    public IValueExpression ValueToSet { get; }
    public string FieldName { get; }

    public StoreFieldOp(IValueExpression thisPtr, IValueExpression valueToSet, string fieldName)
    {
        ThisPtr = thisPtr;
        ValueToSet = valueToSet;
        FieldName = fieldName;
    }

    public override string GenCode()
        => $"{ThisPtr.GenExpressionCode()}.{FieldName} = {ValueToSet.GenExpressionCode()};";
}