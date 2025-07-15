using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.CsharpLang;

public class CsDeclarationsExtraction : IDeclarationsExtraction
{
    public bool IsFileMatcher(string fileName)
    {
        return fileName.EndsWith(".cs");
    }

    public SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData)
    {
        SimpleScanner scanner = CsScanner.Instance;

        Token[] tokens = scanner.Tokenize(fileData, SkipSpaces);
        return BuildDeclarationsFromTokens(tokens, solutionItem);
    }

    private static SolutionIndexItem[] BuildDeclarationsFromTokens(Token[] tokens, SolutionItem solutionItem)
    {
        string[] declarations = Declarations;
        List<SolutionIndexItem> resultList = new List<SolutionIndexItem>();
        for (int index = 1; index < tokens.Length; index++)
        {
            Token nextToken = tokens[index];
            if (nextToken.Kind != TokenKindsCSharp.Identifier)
            {
                continue;
            }

            Token token = tokens[index - 1];
            if (token.Kind != TokenKindsCSharp.Reserved)
            {
                continue;
            }

            if (!IsDeclaration(token, declarations))
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

    private static bool IsDeclaration(Token token, string[] declarations)
        => token.IsInTexts(declarations);

    private static bool SkipSpaces(Token token)
    {
        return token.Kind switch
        {
            TokenKindsCSharp.Spaces => false,
            TokenKindsCSharp.Comment => false,
            _ => true
        };
    }
}