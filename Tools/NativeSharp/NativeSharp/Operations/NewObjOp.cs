using NativeSharp.CodeGen;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

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
    {
        string args = string.Join(", ", Arguments.Select(x => x.GenExpressionCode()));
        return $"{Left.GenExpressionCode()} = clr_new_{Left.ExpressionType.Mangle()}({args});";
    }
}