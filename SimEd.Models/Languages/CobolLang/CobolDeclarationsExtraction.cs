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
        List<SolutionIndexItem>? resultList = null;
        for (int index = 2; index < tokens.Length; index++)
        {
            Token prevToken2 = tokens[index - 2];
            Token prevToken = tokens[index - 1];
            Token currentToken = tokens[index];
            bool tokenMatchPrecondition =
                prevToken2.Kind == TokenKindsCobol.Reserved
                && prevToken.Kind == TokenKindsCobol.Reserved
                && currentToken.Kind == TokenKindsCobol.Identifier;
            if (!tokenMatchPrecondition)
            {
                continue;
            }

            if (!prevToken.IsText("PROGRAM"))
            {
                continue;
            }

            if (!prevToken2.IsText("END"))
            {
                continue;
            }

            resultList ??= [];
            resultList.Add(new SolutionIndexItem(currentToken, solutionItem, currentToken.GetText()));
        }

        return resultList?.ToArray() ?? [];
    }

    private static bool IsDeclaration(Token token, string[] declarations)
        => token.IsInTexts(declarations);

    private static bool SkipSpaces(Token token)
        => token.Kind switch
        {
            TokenKindsCobol.Spaces => false,
            TokenKindsCobol.Comment => false,
            _ => true
        };
}