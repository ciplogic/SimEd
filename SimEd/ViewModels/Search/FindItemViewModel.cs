using CommunityToolkit.Mvvm.ComponentModel;
using SimEd.Models.Languages.Common;

namespace SimEd.ViewModels.Search;

public class FindItemViewModel : ObservableObject
{
    public string FileName  =>GetFileName();

    private string GetFileName()
    {
        return $"{SolutionItem.Token}:{SolutionItem.Kind}";
    }

    public string ClassName {get; set; }  = string.Empty;
    public int LineNumber {get; set;}
    public int ColumnNumber {get; set;}
    public SolutionIndexItem SolutionItem { get; set; }
}