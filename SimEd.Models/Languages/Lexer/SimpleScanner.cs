namespace SimEd.Models.Languages.Lexer;

public class SimpleScanner
{
    public BaseRule[] Rules { get; init; } = [];

    public Token[] Tokenize(ArraySegment<char> segment, Func<Token, bool> tokenFilter)
    {
        List<Token> tokens = [];
        int pos = 0;
        BaseRule[] rules = Rules;
        var originalSegment = segment;

        while (segment.Count > 0)
        {
            Token? foundToken = Match(segment, rules, pos);
            if (foundToken == null)
            {
                Token token = new(segment, pos, "UnparsedToken");
                tokens.Add(token);
                return tokens.ToArray();
            }

            if (tokenFilter(foundToken.Value))
            {
                tokens.Add(foundToken.Value);
            }

            pos += foundToken.Value.Text.Count;
            segment = originalSegment.Slice(pos);
        }

        return tokens.ToArray();
    }

    private static Token? Match(
        ArraySegment<char> segment,
        BaseRule[] rules,
        int pos)
    {
        foreach (var rule in rules)
        {
            int matchLen = rule.Match(segment);
            if (matchLen == 0)
            {
                continue;
            }


            Token token = new Token(segment.Slice(0, matchLen), pos, rule.Kind);
            return token;
        }

        return null;
    }
}