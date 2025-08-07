using System.Runtime.CompilerServices;
using SimEd.Models.Languages.CurlyBasedLanguages;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.CobolLang;

public static class CobolScanner
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
            "ACCEPT", "ACCESS", "ADD", "ADVANCING", "AFTER", "ALL", "ALPHABET",
            "ALPHABETIC", "ALPHABETIC-LOWER", "ALPHABETIC-UPPER", "ALPHANUMERIC",
            "ALPHANUMERIC-EDITED", "ALSO", "ALTER", "ALTERNATE", "AND", "ANY",
            "ARE", "AREA", "AREAS", "AS", "ASCENDING", "ASSIGN", "AT", "AUTHOR",
            "AUTO", "BASIS", "BEFORE", "BEGINNING", "BINARY", "BLANK", "BLOCK",
            "BOTTOM", "BY", "CALL", "CANCEL", "CBL", "CHAIN", "CHANGED", "CHARACTER",
            "CHARACTERS", "CLASS", "CLOSE", "COBOL", "CODE", "CODE-SET", "COLLATING",
            "COLUMN", "COMMA", "COMMON", "COMMUNICATION", "COMP", "COMPUTE", "CONFIGURATION",
            "CONTAINS", "CONTENT", "CONTINUE", "CONTROL", "CONVERTING", "COPY",
            "CORR", "CORRESPONDING", "COUNT", "CURRENCY", "DATA", "DATE", "DAY",
            "DE", "DEBUGGING", "DECIMAL-POINT", "DECLARE", "DELETE", "DELIMITED",
            "DELIMITER", "DEPENDING", "DESCENDING", "DESTINATION", "DETAIL", "DISPLAY",
            "DIVIDE", "DIVISION", "DOWN", "DUPLICATES", "DYNAMIC", "EGI", "ELSE",
            "END-ADD", "END-CALL", "END-COMPUTE", "END-DELETE", "END-DIVIDE",
            "END-EVALUATE", "END-IF", "END-MULTIPLY", "END-PERFORM", "END-READ",
            "END-RETURN", "END-REWRITE", "END-SEARCH", "END-START", "END-STRING",
            "END-SUBTRACT", "END-UNSTRING", "END-WRITE",
            "END",  "ENTRY", "ENVIRONMENT",
            "EOP", "EQUAL", "ERROR", "EVALUATE", "EVERY", "EXCEPTION", "EXCLUSIVE",
            "EXIT", "EXTEND", "EXTERNAL", "FALSE", "FD", "FILE", "FILLER", "FINAL",
            "FIRST", "FOOTING", "FOR", "FROM", "FUNCTION", "GENERATE", "GIVING",
            "GLOBAL", "GO", "GOBACK", "GREATER", "GROUP", "HEADING", "HIGH-VALUE",
            "HIGH-VALUES", "I-O", "I-O-CONTROL", "IDENTIFICATION", "IF", "IN",
            "INDEX", "INDEXED", "INDICATE", "INITIAL", "INITIALIZE", "INITIATE",
            "INPUT", "INSPECT", "INSTALLATION", "INTO", "INVALID", "IS", "JUST",
            "JUSTIFIED", "KEY", "LABEL", "LAST", "LEADING", "LEFT", "LENGTH", "LESS",
            "LIMIT", "LIMITS", "LINAGE", "LINE", "LINES", "LINKAGE", "LOCAL-STORAGE",
            "LOCK", "LOW-VALUE", "LOW-VALUES", "MEMORY", "MERGE", "MESSAGE",
            "MODE", "MODULES", "MOVE", "MULTIPLE", "MULTIPLY", "NATIVE", "NEGATIVE",
            "NEXT", "NO", "NOT", "NULL", "NULLS", "NUMBER", "NUMERIC", "NUMERIC-EDITED",
            "OBJECT-COMPUTER", "OCCURS", "OF", "OFF", "OMITTED", "ON", "OPEN",
            "OPTIONAL", "OR", "ORDER", "ORGANIZATION", "OTHER", "OUTPUT", "OVERFLOW",
            "PACKED-DECIMAL", "PADDING", "PAGE", "PARAGRAPH", "PERFORM", "PF", "PH",
            "PIC", "PICTURE", "PLUS", "POINTER", "POSITION", "POSITIVE", "PROCEDURE",
            "PROCEDURES", "PROCEED", "PROGRAM-ID", "PROGRAM", "QUOTE", "QUOTES",
            "RANDOM", "RD", "READ", "RECEIVE", "RECORD", "RECORDS", "REDEFINES",
            "REEL", "REFERENCE", "REFERENCES", "RELATIVE", "RELEASE", "REMAINDER",
            "REMOVAL", "RENAMES", "REPLACE", "REPLACING", "REPORT", "REPORTING",
            "REPORTS", "REQUIRED", "RERUN", "RESERVE", "RESET", "RETURN", "RETURNING",
            "REVERSE-VIDEO", "REWIND", "REWRITE", "RF", "RH", "RIGHT", "ROUNDED",
            "RUN", "SAME", "SCREEN", "SD", "SEARCH", "SECTION", "SECURITY", "SEGMENT",
            "SEGMENT-LIMIT", "SELECT", "SEND", "SENTENCE", "SEPARATE", "SEQUENCE",
            "SEQUENTIAL", "SET", "SIGN", "SIZE", "SORT", "SOURCE", "SOURCE-COMPUTER",
            "SPACE", "SPACES", "SPECIAL-NAMES", "STANDARD", "START", "STATUS",
            "STOP", "STRING", "SUB-QUEUE-1", "SUB-QUEUE-2", "SUB-QUEUE-3",
            "SUBTRACT", "SUM", "SUPPRESS", "SYMBOLIC", "SYNC", "SYNCHRONIZED",
            "TABLE", "TALLY", "TAPE", "TERMINAL", "TERMINATE", "TEST", "TEXT",
            "THAN", "THEN", "THROUGH", "THRU", "TIME", "TIMES", "TO", "TOP", "TRACE",
            "TRAILING", "TRUE", "TYPE", "UNIT", "UNSTRING", "UNTIL", "UP", "UPON",
            "USAGE", "USE", "USING", "VALUE", "VALUES", "VARYING", "WHEN",
            "WHEN-COMPILED", "WITH", "WORDS", "WORKING-STORAGE", "WRITE", "ZERO",
            "ZEROES", "ZEROS"
        ]);

    private static SimpleScanner BuildScanner()
        => new()
        {
            Rules =
            [
                new LambdaRule(TokenKindsCobol.Reserved, ReservedMatch),
                new LambdaRule(TokenKindsCobol.Identifier, CobolIdentifierMatch),

                new LambdaRule(TokenKindsCobol.Comment, CurlyLexerRules.CommentMatch),
                new LambdaRule(TokenKindsCobol.Operator, OperatorsMatch),
                new LambdaRule(TokenKindsCobol.Spaces, CurlyLexerRules.SpacesMatch),
                new LambdaRule(TokenKindsCobol.Eoln, CurlyLexerRules.EolnMatch),

                new LambdaRule(TokenKindsCobol.Number, CurlyLexerRules.NumberMatch),
                new LambdaRule(TokenKindsCobol.QuotedString, CurlyLexerRules.StringMatch),
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

        int matchIdentifier = CobolIdentifierMatch(segment);
        return matchIdentifier == matchReservedLength
            ? matchReservedLength
            : 0;
    }

    private static int CobolIdentifierMatch(ArraySegment<char> segment)
    {
        var identifierMatchLen = CurlyLexerRules.IdentifierMatch(segment);
        if (identifierMatchLen == 0)
        {
            return 0;
        }

        if (identifierMatchLen == segment.Count)
        {
            return identifierMatchLen;
        }

        if (segment[identifierMatchLen] != '-')
        {
            return identifierMatchLen;
        }
        var secondPartLen = CobolIdentifierMatch(segment.Slice(identifierMatchLen + 1));
        return identifierMatchLen + 1 + secondPartLen;

    }
}