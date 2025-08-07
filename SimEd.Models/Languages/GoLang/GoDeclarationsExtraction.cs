using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.GoLang;

public class GoDeclarationsExtraction : IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        return fileName.EndsWith(".go");
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = GoScanner.Instance;

        Token[] tokens = scanner.Tokenize(fileData, SkipSpaces);
        return BuildDeclarationsFromTokens(tokens, solutionItem);
    }

    private static SolutionIndexItem[] BuildDeclarationsFromTokens(Token[] tokens, SolutionItem solutionItem)
    {
        string[] declarations = Declarations;
        List<SolutionIndexItem>? resultList = null;
        for (int index = 1; index < tokens.Length; index++)
        {
            Token currentToken = tokens[index];
            if (currentToken.Kind != TokenKindsGo.Identifier)
            {
                continue;
            }

            Token prevToken = tokens[index - 1];
            if (prevToken.Kind != TokenKindsGo.Reserved)
            {
                continue;
            }

            if (!IsDeclaration(prevToken, declarations))
            {
                continue;
            }
            resultList ??= [];
            resultList.Add(new SolutionIndexItem(currentToken, solutionItem, prevToken.GetText()));
        }

        return resultList?.ToArray() ?? [];
    }

    private static readonly string[] Declarations =
    [
        "interface", "struct", "type", "func" 
    ];

    private static bool IsDeclaration(Token token, string[] declarations)
        => token.IsInTexts(declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsGo.Spaces => false,
            TokenKindsGo.Comment => false,
            _ => true
        };
    }
}