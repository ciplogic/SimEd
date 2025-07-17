using SimEd.Models.Languages.CsharpLang;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.JsTsLang;

internal static class JsScanner
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
            "class", "interface", "enum",
            "public", "protected", "private",
            "var", "let", "const", "switch",
            "function",
            "return", "abstract", "as", "base", "break", "case", "catch",
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsJs.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsJs.Identifier, CurlyLexerRules.IdentifierMatch),

                new LambdaRule(TokenKindsJs.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsJs.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsJs.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsJs.Eoln, CurlyLexerRules.EolnMatch),
                
                new LambdaRule(TokenKindsJs.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsJs.QuotedString, CurlyLexerRules.StringMatch),
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