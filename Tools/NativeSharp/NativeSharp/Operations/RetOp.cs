using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

internal class RetOp : BaseOp
{
    public IValueExpression? ValueExpression { get; }

    public RetOp(IValueExpression? valueExpression) => ValueExpression = valueExpression;

    public override string GenCode()
        => ValueExpression is null
            ? ""
            : $"return {ValueExpression.Code()};";
}