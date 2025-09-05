using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

internal class NewObjOp : BaseOp
{
    public VReg Result { get; }
    public IValueExpression[] Arguments { get; }

    public NewObjOp(VReg result, IValueExpression[] arguments)
    {
        Result = result;
        Arguments = arguments;
    }

    public override string GenCode()
    {
        string args = string.Join(", ", Arguments.Select(x => x.GenExpressionCode()));
        return $"{Result.GenExpressionCode} = clr_new_obj({args});";
    }
}