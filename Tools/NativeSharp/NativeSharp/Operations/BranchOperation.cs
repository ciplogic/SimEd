using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

internal class BranchOperation : BaseOp
{
    public int Offset { get; }
    public string Name { get; }
    public IValueExpression? Condition { get; }

    public BranchOperation(int offset, string name, IValueExpression? condition)
    {
        Offset = offset;
        Name = name;
        Condition = condition;
    }

    public override string ToString()
        => GenCode();

    public override string GenCode()
    {
        return Condition is null 
            ? $"goto label_{Offset};"
            : $"if ({Name.Mangle()}({Condition})) goto label_{Offset};";
    }
}