using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

internal class ConvOp : BaseOp
{
    private readonly string opName;
    private readonly VReg resultVar;
    private readonly IValueExpression rightSideVar;

    public ConvOp(string opName, VReg resultVar, IValueExpression rightSideVar)
    {
        this.opName = opName;
        this.resultVar = resultVar;
        this.rightSideVar = rightSideVar;
    }

    public override string GenCode()
    {
        return $"{resultVar.GenExpressionCode()} = {opName} ({rightSideVar.GenExpressionCode()});";
    }
}