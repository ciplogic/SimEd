using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.KotlinLang;

public class KotlinDeclarationsExtraction: IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return extension is ".kt" or ".kts";
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = KotlinScanner.Instance;

        Token[] tokens = scanner.Tokenize(fileData, SkipSpaces).ToArray();
        return BuildDeclarationsFromTokens(tokens, solutionItem);
    }

    private static SolutionIndexItem[] BuildDeclarationsFromTokens(Token[] tokens, SolutionItem solutionItem)
    {
        List<SolutionIndexItem> resultList = new List<SolutionIndexItem>();
        for (int index = 0; index < tokens.Length; index++)
        {
            Token token = tokens[index];
            if (!IsDeclaration(token))
            {
                continue;
            }

            Token nextToken = tokens[index + 1];
            if (nextToken.Kind != TokenKindsKotlin.Identifier)
            {
                continue;
            }

            resultList.Add(new SolutionIndexItem(nextToken, solutionItem, token.GetText()));
        }

        return resultList.ToArray();
    }

    private static readonly string[] Declarations =
    [
        "class",
        "object",
        "interface",
        "enum"
    ];

    private static bool IsDeclaration(Token token)
        => token.Kind == TokenKindsKotlin.Reserved
           && token.IsInTexts(Declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsKotlin.Spaces => false,
            TokenKindsKotlin.Comment => false,
            _ => true
        };
    }
}