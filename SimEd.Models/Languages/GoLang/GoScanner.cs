using System.Runtime.CompilerServices;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.GoLang;

internal static class GoScanner
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
            // Declaration keywords
            "var", "const", "type", "func", "package", "import",
            
            // Control flow keywords
            "if", "else", "switch", "case", "default", "for", 
            "break", "continue", "goto", "fallthrough",
            
            // Function-related keywords
            "return", "defer",
            
            // Channel and concurrency keywords
            "go", "chan", "select",
            
            // Interface and type assertion
            "interface", "struct", "map", "range",
            
            // Error handling
            "error",
            
            // Constants
            "true", "false", "iota", "nil"
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsGo.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsGo.Identifier, CurlyLexerRules.IdentifierMatch),

                new LambdaRule(TokenKindsGo.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsGo.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsGo.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsGo.Eoln, CurlyLexerRules.EolnMatch),
                
                new LambdaRule(TokenKindsGo.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsGo.QuotedString, CurlyLexerRules.StringMatch),
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