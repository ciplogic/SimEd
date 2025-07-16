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
        List<SolutionIndexItem>? resultList = null;
        for (int index = 1; index < tokens.Length; index++)
        {
            Token currentToken = tokens[index];
            if (currentToken.Kind != TokenKindsCSharp.Identifier)
            {
                continue;
            }

            Token prevToken = tokens[index - 1];
            if (prevToken.Kind != TokenKindsCSharp.Reserved)
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