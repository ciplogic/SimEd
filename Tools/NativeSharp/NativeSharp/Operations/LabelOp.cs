namespace NativeSharp.Operations;

internal class LabelOp : BaseOp
{
    public int Offset { get; }

    public LabelOp(int offset)
        => Offset = offset;

    public override string ToString() => GenCode();

    public override string GenCode()
        => $"label_{Offset}:";
}