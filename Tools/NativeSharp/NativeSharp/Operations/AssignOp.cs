using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

class AssignOp : BaseOp
{
    public IRefValue Left { get; set; }
    public IValueExpression Expression { get; set; }

    public override string GenCode()
        => $"{Left.GenCode()} = {Expression.GenExpressionCode()};";
}