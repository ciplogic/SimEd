using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

public class LdLenOp : BaseOp
{
    private readonly IndexedVariable left;
    private readonly IndexedVariable right;

    public LdLenOp(IndexedVariable left, IndexedVariable right)
    {
        this.left = left;
        this.right = right;
    }

    public override string GenCode()
    {
        return $"{left.GenCode()} = {right.GenCode()}->size();";
    }
}