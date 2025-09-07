using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class StoreFieldOp : BaseOp
{
    public IndexedVariable ThisPtr { get; }
    public IValueExpression ValueToSet { get; }
    public string FieldName { get; }

    public StoreFieldOp(IndexedVariable thisPtr, IValueExpression valueToSet, string fieldName)
    {
        ThisPtr = thisPtr;
        ValueToSet = valueToSet;
        FieldName = fieldName;
    }

    public override string GenCode()
    {
        var type = ThisPtr.ExpressionType;
        var isByRef = !type.IsValueType;
        var derefText = isByRef ? "->" : ".";
        return $"{ThisPtr.Code()}{derefText}{FieldName} = {ValueToSet.Code()};";
    }
}