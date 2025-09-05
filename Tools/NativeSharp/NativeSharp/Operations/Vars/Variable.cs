using NativeSharp.Operations.Common;

namespace NativeSharp.Operations.Vars;

public abstract class Variable : IRefValue, IValueExpression
{
    public Type ExpressionType { get; set; }

    public string GenExpressionCode()
    {
        return GenCodeImpl();
    }

    public string GenCode()
    {
        return GenCodeImpl();
    }

    public abstract string GenCodeImpl();
}