using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

class AssignOp : LeftOp
{
    public IValueExpression Expression { get; set; }

    public AssignOp(IndexedVariable left, IValueExpression expression) : base(left)
    {
        Expression = expression;
    }

    public override string GenCode()
        => $"{Left.GenCode()} = {Expression.Code()};";
}