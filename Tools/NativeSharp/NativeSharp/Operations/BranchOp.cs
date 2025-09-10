using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

internal class BranchOp : BaseOp
{
    public int Offset { get; }
    public string Name { get; }
    public IValueExpression Condition { get; set; }

    public BranchOp(int offset, string name, IValueExpression condition)
    {
        Offset = offset;
        Name = name;
        Condition = condition;
    }

    public override string ToString()
        => GenCode();

    public override string GenCode()
        => $"if ({Name.Mangle()}({Condition.Code()})) goto label_{Offset};";
}