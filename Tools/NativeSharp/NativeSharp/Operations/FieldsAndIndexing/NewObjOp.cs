using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations.FieldsAndIndexing;

internal class NewObjOp : LeftOp
{
    public IValueExpression[] Arguments { get; }

    public NewObjOp(VReg left, IValueExpression[] arguments)
        : base(left)
    {
        Arguments = arguments;
    }

    public override string GenCode()
    {
        var expressionTypeName = Left.ExpressionType.Mangle(RefKind.Value);
        return $"{Left.Code()} = new_ref<{expressionTypeName}>();";
    }
}