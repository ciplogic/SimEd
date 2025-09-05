using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

class BinaryOp : BaseOp
{
    public string Operator { get; set; } = null!;
    public IRefValue Left { get; set; }
    public IValueExpression LeftExpression { get; set; }
    public IValueExpression RightExpression { get; set; }

    public override string GenCode()
        => $"{Left.GenCode()} = {Operator} ({LeftExpression.GenExpressionCode()}, {RightExpression.GenExpressionCode()});";
}