namespace NativeSharp.Operations;

public abstract class BaseOp
{
    public abstract string GenCode();

    public override string ToString() => GenCode();
}