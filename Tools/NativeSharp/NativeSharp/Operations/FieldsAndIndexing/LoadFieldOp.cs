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
        Type type = ThisPtr.ExpressionType;
        bool isByRef = !type.IsValueType;
        string derefText = isByRef ? "->" : ".";
        return $"{ResultVar.GenCode()} = {ThisPtr.Code()}{derefText}{FieldName};";
    }
}