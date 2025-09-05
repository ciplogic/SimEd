using NativeSharp.Operations.Common;

namespace NativeSharp.Operations.Values;

internal class ConstantValueExpression(object value) : IValueExpression
{
    public object Value { get; } = value;
    public Type ExpressionType { get; set; }

    public string GenExpressionCode()
    {
        ArgumentNullException.ThrowIfNull(Value);
        if (Value is string text)
        {
            var index = StringPool.Instance.GetIndex(text);
            return $"_clr_str({index})";
        }

        return Value.ToString();
    }

    public static ConstantValueExpression Create(object value) => new(value)
    {
        ExpressionType = value.GetType()
    };
}