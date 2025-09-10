using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

internal class NewObjOp : BaseOp
{
    public VReg Left { get; }
    public IValueExpression[] Arguments { get; }

    public NewObjOp(VReg left, IValueExpression[] arguments)
    {
        Left = left;
        Arguments = arguments;
    }

    public override string GenCode() 
        => $"{Left.Code()} = Ref(new {Left.ExpressionType.Mangle(RefKind.Value)});";
}