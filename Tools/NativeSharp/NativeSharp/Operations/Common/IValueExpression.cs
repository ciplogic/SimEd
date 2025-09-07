namespace NativeSharp.Operations.Common;

public interface IValueExpression
{
    public Type ExpressionType { get; set; }
    string Code();
}