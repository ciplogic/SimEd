using SimEd.Models.Languages.CobolLang;
using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.CsharpLang;
using SimEd.Models.Languages.GoLang;
using SimEd.Models.Languages.JavaLang;
using SimEd.Models.Languages.JsTsLang;
using SimEd.Models.Languages.KotlinLang;
using SimEd.Models.Languages.PythonLanguage;
using SimEd.Models.Languages.RustLang;
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
            new JavaDeclarationsExtraction(),
            new JsDeclarationsExtraction(),
            new KotlinDeclarationsExtraction(),
            new PythonDeclarationsExtraction(), 
            new RustDeclarationsExtraction(),
            new GoDeclarationsExtraction(),
            new CobolDeclarationsExtraction(),
        ];
        return extractions;
    }

    public async Task<SolutionIndexItem[]> Parse(SolutionItem solutionItem)
    {
        IDeclarationsExtraction? extraction = Extractions
            .AsValueEnumerable()
            .FirstOrDefault(it => it.IsFileMatcher(solutionItem.Path.ToString()));
        if (extraction == null)
        {
            return [];
        }

        string dataBytes = await File.ReadAllTextAsync(solutionItem.Path.ToString())
            .ConfigureAwait(false);
        SolutionIndexItem[] items = extraction.ExtractFileDefinitions(
            solutionItem,
            dataBytes.ToCharArray());
        return items;
    }
}