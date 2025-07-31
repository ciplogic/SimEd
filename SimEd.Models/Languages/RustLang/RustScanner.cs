using System.Runtime.CompilerServices;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.RustLang;

internal static class RustScanner
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
            // Reserved keywords used in Rust
            "as", "break", "const", "continue", "crate", "else", "enum", "extern", "false",
            "fn", "for", "if", "impl", "in", "let", "loop", "match", "mod", "move", "mut",
            "pub", "ref", "return", "self", "Self", "static", "struct", "super", "trait",
            "true", "type", "unsafe", "use", "where", "while",

            // Reserved but not currently used
            "abstract", "become", "box", "do", "final", "macro", "override", "priv",
            "try", "typeof", "unsized", "virtual", "yield",

            // Special lifetime-related keyword
            "'static"
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsRust.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsRust.Identifier, CurlyLexerRules.IdentifierMatch),
                new LambdaRule(TokenKindsRust.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsRust.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsRust.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsRust.Eoln, CurlyLexerRules.EolnMatch),
                new LambdaRule(TokenKindsRust.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsRust.QuotedString, CurlyLexerRules.StringMatch),
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