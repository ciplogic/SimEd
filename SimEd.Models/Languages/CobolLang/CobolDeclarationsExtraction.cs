using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.CobolLang;

public class CobolDeclarationsExtraction : IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        return fileName.EndsWith(".cob");
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = CobolScanner.Instance;

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
            if (currentToken.Kind != TokenKindsCobol.Identifier)
            {
                continue;
            }

            Token prevToken = tokens[index - 1];
            if (prevToken.Kind != TokenKindsCobol.Reserved)
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
        "PROCEDURE" 
    ];

    private static bool IsDeclaration(Token token, string[] declarations)
        => token.IsInTexts(declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsCobol.Spaces => false,
            TokenKindsCobol.Comment => false,
            _ => true
        };
    }
}