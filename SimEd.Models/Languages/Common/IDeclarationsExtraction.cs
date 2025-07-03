namespace SimEd.Models.Languages.Common;

public interface IDeclarationsExtraction
{
    bool IsFileMatcher(string fileName);
    
    SolutionIndexItem[] ExtractFileDefinitions(SolutionItem solutionItem, char[] fileData);
}
