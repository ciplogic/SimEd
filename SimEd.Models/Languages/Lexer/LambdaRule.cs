namespace SimEd.Models.Languages.Lexer;

public struct LambdaRule
{
    private readonly Func<ArraySegment<char>, int> _func;

    public string Kind { get; }

    public LambdaRule(string kind, Func<ArraySegment<char>, int> func)
    {
        _func = func;
        Kind = kind;
    }

    public int Match(ArraySegment<char> segment)
        => _func(segment);
}