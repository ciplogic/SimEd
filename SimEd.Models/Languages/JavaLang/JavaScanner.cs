using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.CsharpLang;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.JavaLang;

internal static class JavaScanner
{
    public static SimpleScanner Instance { get; } = BuildScanner();

    private static WordsIndex BuildOperatorsArray()
        => CurlyLexerRules.BuildWordsIndex([
            ".", ",", ";", ":", "%", "^",
            "~",
            "+=", "-=", "*=", "/=",
            "+", "-", "*", "/",
            "==",
            "!", "?",
            ">=", "<=", "<", ">",
            "=>",
            "$",
            "&&", "&", "||", "|",
            "(", ")",
            "[", "]",
            "{", "}",
            "=",

            "@",
        ]);


    private static WordsIndex BuildReservedWordsArray()
        => new([
            "class", "record", "interface", "enum",
            "public", "protected", "private",
            "package",
            "return", "abstract", "as", "base", "break", "case", "catch",
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsCSharp.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsCSharp.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsCSharp.Identifier, CurlyLexerRules.IdentifierMatch),
                new LambdaRule(TokenKindsCSharp.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsCSharp.Eoln, CurlyLexerRules.EolnMatch),
                
                new LambdaRule(TokenKindsCSharp.Number, NumberMatch),
                new LambdaRule(TokenKindsCSharp.QuotedString, CurlyLexerRules.StringMatch),

                new LambdaRule(TokenKindsCSharp.Comment, CurlyLexerRules.CommentMatch),
            ]
        };

    private static readonly WordsIndex Operators = BuildOperatorsArray();

    private static int OperatorsMatch(ArraySegment<char> arg)
        => CurlyLexerRules.MatchArrayOfWordsLength(arg, Operators);


    private static readonly WordsIndex ReservedWords = BuildReservedWordsArray();

    private static int ReservedMatch(ArraySegment<char> segment)
    {
        int matchReservedLength = ReservedWords.MatchLen(segment);
        if (matchReservedLength == 0)
        {
            return 0;
        }

        int matchIdentifier = CurlyLexerRules.IdentifierMatch(segment);
        if (matchIdentifier == 0)
        {
            return 0;
        }

        return matchIdentifier == matchReservedLength
            ? matchReservedLength
            : 0;
    }


    private static int NumberMatch(ArraySegment<char> segment)
        => segment.MatchInSegmentByLambda(IsMatchForNumber);

    static bool IsMatchForNumber(char c)
        => Char.IsDigit(c);
}