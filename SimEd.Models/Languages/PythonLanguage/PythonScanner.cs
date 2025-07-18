using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.PythonLanguage;

internal static class PythonScanner
{
    public static SimpleScanner Instance { get; } = BuildScanner();

    private static WordsIndex BuildOperatorsArray()
        => new([
            "and", "or", "not",
            "+", "**", "//", "-", "*", "/", "%",
            "==", "!=", ">", "<", ">=", "<=",
            "&", "|", "^", "~", "<<", ">>",
            "=", "+=", "-=", "*=", "/=", "%=", "**=", "//=", "&=", "|=", "^=", "<<=", ">>=",
            "(", ")",
            "[", "]",
            "{", "}", "\\",
            "in", "not in",
            "is", "is not",
            "@",
            ".", ":", ","
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
                new LambdaRule(TokenKindsPython.Comment, PythonCommentMatch),
                new LambdaRule(TokenKindsPython.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsPython.Eoln, CurlyLexerRules.EolnMatch),
                new LambdaRule(TokenKindsPython.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsPython.QuotedString, PythonStringMatch),
            ]
        };

    private static int PythonStringMatch(ArraySegment<char> text)
    {
        var firstChar = text[0];
        if (firstChar != '"' && firstChar != '\'')
        {
            return 0;
        }

        int matchMultiLines = PythonStringMatchMultilines(text);
        if (matchMultiLines != 0)
        {
            return matchMultiLines;
        }


        for (int i = 1; i < text.Count - 3; i++)
        {
            if (text[i] == firstChar)
            {
                return i + 1;
            }
        }

        return text.Count;
    }

    private static int PythonStringMatchMultilines(ArraySegment<char> text)
    {
        if (text.Count < 6)
        {
            return 0;
        }

        var firstChar = text[0];

        if (text[1] != firstChar || text[2] != firstChar)
        {
            return 0;
        }

        for (int i = 3; i < text.Count - 3; i++)
        {
            if (text[i] != firstChar)
            {
                continue;
            }

            if (text[i + 1] != firstChar)
            {
                continue;
            }

            if (text[i + 2] != firstChar)
            {
                continue;
            }

            return i + 3;
        }

        return text.Count;
    }


    private static int PythonCommentMatch(ArraySegment<char> text)
    {
        if (text[0] != '#')
        {
            return 0;
        }

        for (int i = 1; i < text.Count; i++)
        {
            if (text[i] == '\n' || text[i] == '\r')
            {
                return i + 1;
            }
        }

        return text.Count;
    }

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