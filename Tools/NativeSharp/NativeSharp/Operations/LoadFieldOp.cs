using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

internal class LoadFieldOp : BaseOp
{
    public IValueExpression ThisPtr { get; }
    public string FieldName { get; }
    public VReg ResultVar { get; }

    public LoadFieldOp(IValueExpression thisPtr, string fieldName, VReg resultVar)
    {
        ThisPtr = thisPtr;
        FieldName = fieldName;
        ResultVar = resultVar;
    }

    public override string GenCode()
    {
        return $"{ResultVar.GenCode()} = {ThisPtr.GenExpressionCode()}.{FieldName};";
    }
}