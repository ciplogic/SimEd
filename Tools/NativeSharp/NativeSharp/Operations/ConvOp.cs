using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

internal class ConvOp(string opName, VReg resultVar, IValueExpression rightSideVar)
    : BaseOp
{
    public string OpName { get; } = opName.Replace('.', '_');
    public VReg ResultVar { get; } = resultVar;
    public IValueExpression RightSideVar { get; } = rightSideVar;

    public override string GenCode()
    {
        return $"{ResultVar.Code()} = {OpName} ({RightSideVar.Code()});";
    }
}