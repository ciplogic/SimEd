using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.PythonLanguage;

public class PythonDeclarationsExtraction : IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return extension is ".py";
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = PythonScanner.Instance;

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
            if (nextToken.Kind != TokenKindsPython.Identifier)
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
        "def"
    ];

    private static bool IsDeclaration(Token token)
        => token.Kind == TokenKindsPython.Reserved
           && token.IsInTexts(Declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsPython.Spaces => false,
            TokenKindsPython.Comment => false,
            _ => true
        };
    }
}