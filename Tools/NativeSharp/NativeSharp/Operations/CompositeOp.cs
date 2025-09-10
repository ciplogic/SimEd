namespace NativeSharp.Operations;

class CompositeOp : BaseOp
{
    public BaseOp[] Ops { get; }

    public CompositeOp(BaseOp[] ops)
    {
        Ops = ops;
    }

    public override string GenCode()
    {
        return string.Join("\n", Ops.Select(op => op.GenCode()));
    }
}