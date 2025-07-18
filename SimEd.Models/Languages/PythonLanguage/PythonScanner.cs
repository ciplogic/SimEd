using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.PythonLanguage;

internal static class PythonScanner
{
    public static SimpleScanner Instance { get; } = BuildScanner();

    private static WordsIndex BuildOperatorsArray()
        => new([
            // Arithmetic
            "+", "-", "*", "/", "%", "**", "//",
            // Comparison
            "==", "!=", ">", "<", ">=", "<=",
            // Logical
            "and", "or", "not",
            // Bitwise
            "&", "|", "^", "~", "<<", ">>",
            // Assignment
            "=", "+=", "-=", "*=", "/=", "%=", "**=", "//=", "&=", "|=", "^=", "<<=", ">>=",
            // Membership
            "in", "not in",
            // Identity
            "is", "is not"
        ]);


    private static WordsIndex BuildReservedWordsArray()
        => new([
          
            "False", "None", "True", "and", "as", "assert", "async", "await",
            "break", "class", "continue", "def", "del", "elif", "else", "except",
            "finally", "for", "from", "global", "if", "import", "in", "is",
            "lambda", "nonlocal", "not", "or", "pass", "raise", "return",
            "try", "while", "with", "yield"
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsPython.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsPython.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsPython.Identifier, CurlyLexerRules.IdentifierMatch),
                new LambdaRule(TokenKindsPython.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsPython.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsPython.Eoln, CurlyLexerRules.EolnMatch),
                new LambdaRule(TokenKindsPython.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsPython.QuotedString, CurlyLexerRules.StringMatch),
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