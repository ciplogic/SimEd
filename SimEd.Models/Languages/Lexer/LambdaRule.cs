namespace SimEd.Models.Languages.Lexer;

public readonly struct LambdaRule(int kind, Func<ArraySegment<char>, int> func)
{
    public int Kind { get; } = kind;

    public int Match(ArraySegment<char> segment)
        => func(segment);
}