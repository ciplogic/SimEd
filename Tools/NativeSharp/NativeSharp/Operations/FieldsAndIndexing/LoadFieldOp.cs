using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class LoadFieldOp : LeftOp
{
    public IndexedVariable ThisPtr { get; }
    public string FieldName { get; }

    public LoadFieldOp(IndexedVariable thisPtr, string fieldName, VReg resultVar)
    : base(resultVar)
    {
        ThisPtr = thisPtr;
        FieldName = fieldName;
    }

    public override string GenCode()
    {
        Type type = ThisPtr.ExpressionType;
        bool isByRef = !type.IsValueType;
        string derefText = isByRef ? "->" : ".";
        return $"{Left.GenCode()} = {ThisPtr.Code()}{derefText}{FieldName};";
    }
}