using System.Diagnostics;
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
                new LambdaRule(TokenKindsCSharp.Spaces, SpacesMatch),
                new LambdaRule(TokenKindsCSharp.Eoln, EolnMatch),
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

    private static int SpacesMatch(ArraySegment<char> segment)
        => segment.MatchInSegmentByLambda(c => c == ' ' || c == '\t');

    private static int EolnMatch(ArraySegment<char> segment)
        => segment.MatchInSegmentByLambda(c => c == '\n' || c == '\r');


    private static readonly WordsIndex ReservedWordsIndex = BuildReservedWordsIndex();

    private static int ReservedMatch(ArraySegment<char> segment)
    {
        var matchReservedLength = ReservedWordsIndex.MatchLen(segment);
        if (matchReservedLength == 0)
        {
            return 0;
        }

        var matchIdentifier = CurlyLexerRules.IdentifierMatch(segment);
        return matchIdentifier == matchReservedLength 
            ? matchReservedLength
            : 0;
    }


    private static int NumberMatch(ArraySegment<char> segment)
        => segment.MatchInSegmentByLambda(IsMatchForNumber);


    static bool IsMatchForNumber(char c)
        => Char.IsDigit(c);
}