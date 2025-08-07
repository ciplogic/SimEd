using Shouldly;
using SimEd.Models.Languages.CobolLang;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Test.Lexer;

public class CobolLexerShould
{
    [Theory]
    [InlineData("PROGRAM-ID")]
    public void MatchTokenFully(string tokenText)
    {
        SimpleScanner cobolScanner = CobolScanner.Instance;
        var cobolLexerTokens = cobolScanner.Tokenize(tokenText.ToCharArray(), _ => true);
        cobolLexerTokens.Length.ShouldBe(1);
        cobolLexerTokens[0].Text.ShouldBe(tokenText);
    }
}