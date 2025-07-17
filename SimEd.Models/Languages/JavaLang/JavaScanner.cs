using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.JavaLang;

internal static class JavaScanner
{
    public static SimpleScanner Instance { get; } = BuildScanner();

    private static WordsIndex BuildOperatorsArray()
        => new([
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
                new LambdaRule(TokenKindsJava.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsJava.Identifier, CurlyLexerRules.IdentifierMatch),
                new LambdaRule(TokenKindsJava.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsJava.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsJava.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsJava.Eoln, CurlyLexerRules.EolnMatch),
                new LambdaRule(TokenKindsJava.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsJava.QuotedString, CurlyLexerRules.StringMatch),
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
}