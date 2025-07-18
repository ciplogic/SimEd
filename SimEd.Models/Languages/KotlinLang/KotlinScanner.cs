using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.KotlinLang;

internal static class KotlinScanner
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
            // Hard keywords
            "as", "break", "class", "continue", "do", "else", "false", "for",
            "fun", "if", "in", "interface", "is", "null", "object", "package",
            "return", "super", "this", "throw", "true", "try", "typealias",
            "typeof", "val", "var", "when", "while",

            // Soft keywords (used in specific contexts)
            "by", "catch", "constructor", "delegate", "dynamic", "field", "file",
            "finally", "get", "import", "init", "param", "property", "receiver",
            "set", "setparam", "where", "actual", "abstract", "annotation",
            "companion", "const", "crossinline", "data", "enum", "expect",
            "external", "final", "infix", "inline", "inner", "internal", "lateinit",
            "noinline", "open", "operator", "out", "override", "private",
            "protected", "public", "reified", "sealed", "suspend", "tailrec",
            "vararg"
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsKotlin.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsKotlin.Identifier, CurlyLexerRules.IdentifierMatch),
                new LambdaRule(TokenKindsKotlin.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsKotlin.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsKotlin.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsKotlin.Eoln, CurlyLexerRules.EolnMatch),
                new LambdaRule(TokenKindsKotlin.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsKotlin.QuotedString, CurlyLexerRules.StringMatch),
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