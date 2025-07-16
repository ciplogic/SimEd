using System.Runtime.CompilerServices;
using SimEd.Models.Languages.CsharpLang;

namespace SimEd.Models.Languages.Lexer;

public class SimpleScanner
{
    public LambdaRule[] Rules { get; init; } = [];

    public IEnumerable<Token> EnumerateTokens(ArraySegment<char> segment)
    {
        int pos = 0;
        LambdaRule[] rules = Rules;
        ArraySegment<char> originalSegment = segment;

        while (segment.Count > 0)
        {
            Token? foundToken = Match(segment, rules, pos);
            if (foundToken == null)
            {
                Token token = new( TokenKindsCSharp.Unknown, segment, pos);
                yield return (token);
                yield break;
            }

            {
                yield return (foundToken.Value);
            }

            pos += foundToken.Value.Text.Count;
            segment = originalSegment.Slice(pos);
        }
    }

    public Token[] Tokenize(ArraySegment<char> segment, Func<Token, bool> tokenFilter)
    {
        List<Token> tokens = [];
        int pos = 0;
        LambdaRule[] rules = Rules;
        ArraySegment<char> originalSegment = segment;

        while (segment.Count > 0)
        {
            Token? foundToken = Match(segment, rules, pos);
            if (foundToken == null)
            {
                Token token = new(TokenKindsCSharp.Unknown, segment, pos);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static Token? Match(
        ArraySegment<char> segment,
        LambdaRule[] rules,
        int pos)
    {
        foreach (LambdaRule rule in rules)
        {
            int matchLen = rule.Match(segment);
            if (matchLen == 0)
            {
                continue;
            }


            Token token = new (rule.Kind, segment.Slice(0, matchLen), pos);
            return token;
        }

        return null;
    }
}