using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.JavaLang;

public class JavaDeclarationsExtraction: IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        return fileName.EndsWith(".java");
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = JavaScanner.Instance;

        var tokens = scanner.Tokenize(fileData, SkipSpaces).ToArray();
        return BuildDeclarationsFromTokens(tokens, solutionItem);
    }

    private static SolutionIndexItem[] BuildDeclarationsFromTokens(Token[] tokens, SolutionItem solutionItem)
    {
        var resultList = new List<SolutionIndexItem>();
        for (var index = 0; index < tokens.Length; index++)
        {
            var token = tokens[index];
            if (!IsDeclaration(token))
            {
                continue;
            }

            Token nextToken = tokens[index + 1];
            if (nextToken.Kind != TokenKindsJava.Identifier)
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
        "struct",
        "record",
        "interface",
        "enum"
    ];

    private static bool IsDeclaration(Token token)
        => token.Kind == TokenKindsJava.Reserved
           && token.IsInTexts(Declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsJava.Spaces => false,
            TokenKindsJava.Comment => false,
            _ => true
        };
    }
}