using System.Diagnostics;
using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.CsharpLang;

internal static class CsScanner
{
    public static SimpleScanner Instance { get; } = BuildScanner();

    private static WordsIndex BuildOperatorsIndex()
        => new WordsIndex([
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
        ]);

    private static char[][] BuildReservedWordsArray()
        => CurlyLexerRules.BuildCharsArrays([
            "class", "record", "interface", "struct", "enum", "delegate",
            "public", "protected", "internal", "private",
            "namespace", "using",
            "return", "abstract", "as", "base", "break", "case", "catch",
        ]);

    private static WordsIndex BuildReservedWordsIndex()
        => CurlyLexerRules.BuildWordsIndex([
            "class", "record", "interface", "struct", "enum", "delegate",
            "public", "protected", "internal", "private",
            "namespace", "using",
            "return", "abstract", "as", "base", "break", "case", "catch",
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsCSharp.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsCSharp.Eoln, CurlyLexerRules.EolnMatch),
                new LambdaRule(TokenKindsCSharp.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsCSharp.QuotedString, CurlyLexerRules.StringMatch),
                new LambdaRule(TokenKindsCSharp.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsCSharp.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsCSharp.Identifier, CurlyLexerRules.IdentifierMatch),
                new LambdaRule(TokenKindsCSharp.Number, NumberMatch),
            ]
        };

    private static readonly WordsIndex OperatorsIndex = BuildOperatorsIndex();

    private static int OperatorsMatch(ArraySegment<char> arg) => OperatorsIndex.MatchLen(arg);

    private static readonly WordsIndex ReservedWordsIndex = BuildReservedWordsIndex();

    private static int ReservedMatch(ArraySegment<char> segment)
    {
        int matchReservedLength = ReservedWordsIndex.MatchLen(segment);
        if (matchReservedLength == 0)
        {
            return 0;
        }

        int matchIdentifier = CurlyLexerRules.IdentifierMatch(segment);
        return matchIdentifier == matchReservedLength
            ? matchReservedLength
            : 0;
    }


    private static int NumberMatch(ArraySegment<char> segment)
    {
        if (!Char.IsDigit(segment[0]))
        {
            return 0;
        }
        return segment.MatchInSegmentByLambda(IsMatchForNumber);
    }


    static bool IsMatchForNumber(char c)
        => Char.IsDigit(c);
}