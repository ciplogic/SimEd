using NativeSharp.CodeGen;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

internal class NewArrayOp : BaseOp
{
    public VReg Result { get; }
    public Type ElementType { get; }
    public IValueExpression Count { get; }

    public NewArrayOp(VReg result, Type elementType, IValueExpression count)
    {
        Result = result;
        ElementType = elementType;
        Count = count;
    }

    public override string GenCode() 
        => $"{Result.GenCode()} = new_arr<{ElementType.Mangle()}>({Count.GenExpressionCode()});";
}