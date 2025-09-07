using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class LoadFieldOp : BaseOp
{
    public IndexedVariable ThisPtr { get; }
    public string FieldName { get; }
    public VReg ResultVar { get; }

    public LoadFieldOp(IndexedVariable thisPtr, string fieldName, VReg resultVar)
    {
        ThisPtr = thisPtr;
        FieldName = fieldName;
        ResultVar = resultVar;
    }

    public override string GenCode()
    {
        var type = ThisPtr.ExpressionType;
        var isByRef = !type.IsValueType;
        var derefText = isByRef ? "->" : ".";
        return $"{ResultVar.GenCode()} = {ThisPtr.GenExpressionCode()}{derefText}{FieldName};";
    }
}