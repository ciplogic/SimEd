namespace NativeSharp.Operations;

public abstract class BaseOp
{
    public abstract string GenCode();

    override public string ToString() => GenCode();
}