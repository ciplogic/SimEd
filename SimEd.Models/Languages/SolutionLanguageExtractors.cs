using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.CsharpLang;
using SimEd.Models.Languages.JavaLang;
using ZLinq;

namespace SimEd.Models.Languages;

public class SolutionLanguageExtractors
{
    public IDeclarationsExtraction[] Extractions { get; set; } = BuildDefaultList();

    private static IDeclarationsExtraction[] BuildDefaultList()
    {
        IDeclarationsExtraction[] extractions =
        [
            new CsDeclarationsExtraction(),
            new JavaDeclarationsExtraction()
        ];
        return extractions;
    }

    public async Task<SolutionIndexItem[]> Parse(SolutionItem solutionItem)
    {
        IDeclarationsExtraction? extraction = Extractions
            .AsValueEnumerable()
            .FirstOrDefault(it => it.IsFileMatcher(solutionItem.Path));
        if (extraction == null)
        {
            return [];
        }

        string dataBytes = await File.ReadAllTextAsync(solutionItem.Path)
            .ConfigureAwait(false);
        SolutionIndexItem[] items = extraction.ExtractFileDefinitions(
            solutionItem,
            dataBytes.ToCharArray());
        return items;
    }
}