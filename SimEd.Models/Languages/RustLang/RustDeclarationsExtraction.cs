using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.RustLang;

public class RustDeclarationsExtraction : IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        return fileName.EndsWith(".rs");
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = RustScanner.Instance;

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
            if (currentToken.Kind != TokenKindsRust.Identifier)
            {
                continue;
            }

            Token prevToken = tokens[index - 1];
            if (prevToken.Kind != TokenKindsRust.Reserved)
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
        "fn"
    ];

    private static bool IsDeclaration(Token token, string[] declarations)
        => token.IsInTexts(declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsRust.Spaces => false,
            TokenKindsRust.Comment => false,
            _ => true
        };
    }
}