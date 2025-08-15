namespace SimEd.Models.Languages.CsharpLang;

internal static class TokenKindsCSharp
{
    public const int Unknown = 0;
    public const int Spaces = 'S';
    public const int Eoln = 'E';
    public const int Operator = 'O';
    public const int Identifier = 'I';
    public const int Reserved = 'R';
    public const int Number = 'N';
    public const int QuotedString = 'Q';
    public const int Comment = 'C';
}