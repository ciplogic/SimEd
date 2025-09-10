using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

class BinaryOp : LeftOp
{
    public BinaryOp(IndexedVariable left) : base(left)
    {
    }

    public string Operator { get; set; } = null!;
    public IValueExpression LeftExpression { get; set; }
    public IValueExpression RightExpression { get; set; }

    public override string GenCode()
        => $"{Left.GenCode()} = {Operator} ({LeftExpression.Code()}, {RightExpression.Code()});";
}