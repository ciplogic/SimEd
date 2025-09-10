namespace NativeSharp.Operations;

internal class GotoOp : BaseOp
{
    public int Offset { get; }

    public GotoOp(int offset) => Offset = offset;

    public override string GenCode() 
        => $"goto label_{Offset};";
}