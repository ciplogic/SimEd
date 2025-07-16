using System.Runtime.CompilerServices;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.CsharpLang;

internal static class CsScanner
{
    public static SimpleScanner Instance { get; } = BuildScanner();

    private static WordsIndex BuildOperatorsIndex()
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
        ]);

    private static WordsIndex BuildReservedWordsIndex()
        => new([
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
                new LambdaRule(TokenKindsCSharp.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsCSharp.Identifier, CurlyLexerRules.IdentifierMatch),

                new LambdaRule(TokenKindsCSharp.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsCSharp.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsCSharp.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsCSharp.Eoln, CurlyLexerRules.EolnMatch),
                
                new LambdaRule(TokenKindsCSharp.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsCSharp.QuotedString, CurlyLexerRules.StringMatch),
            ]
        };

    private static readonly WordsIndex OperatorsIndex = BuildOperatorsIndex();

    private static int OperatorsMatch(ArraySegment<char> arg) => OperatorsIndex.MatchLen(arg);

    private static readonly WordsIndex ReservedWordsIndex = BuildReservedWordsIndex();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int ReservedMatch(ArraySegment<char> segment)
    {
        int matchReservedLength = ReservedWordsIndex.MatchLen(segment);
        if (matchReservedLength == 0)
        {
            return 0;
        }

        int matchIdentifier = CurlyLexerRules.IdentifierMatch(segment.Slice(matchReservedLength));
        return matchIdentifier == 0
            ? matchReservedLength
            : 0;
    }
    
}