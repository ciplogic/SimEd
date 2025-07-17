using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.JavaLang;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.JsTsLang;

public class JsDeclarationsExtraction: IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        return fileName.EndsWith(".js")|| fileName.EndsWith(".jsx")
            || fileName.EndsWith(".ts")|| fileName.EndsWith(".tsx");
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = JsScanner.Instance;

        Token[] tokens = scanner.Tokenize(fileData, SkipSpaces).ToArray();
        return BuildDeclarationsFromTokens(tokens, solutionItem);
    }

    private static SolutionIndexItem[] BuildDeclarationsFromTokens(Token[] tokens, SolutionItem solutionItem)
    {
        List<SolutionIndexItem> resultList = [];
        for (int index = 0; index < tokens.Length; index++)
        {
            Token token = tokens[index];
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
        "function",
        "interface",
        "enum"
    ];

    private static bool IsDeclaration(Token token)
        => token.Kind == TokenKindsJava.Reserved
           && token.IsInTexts(Declarations);

    private static bool SkipSpaces(Token token) =>
        token.Kind switch
        {
            TokenKindsJava.Spaces => false,
            TokenKindsJava.Comment => false,
            _ => true
        };
}