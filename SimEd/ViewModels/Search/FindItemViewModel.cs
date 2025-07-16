using CommunityToolkit.Mvvm.ComponentModel;
using SimEd.Models.Languages.Common;

namespace SimEd.ViewModels.Search;

public class FindItemViewModel : ObservableObject
{
    public string FileName => GetFileName();

    private string GetFileName() => $"{SolutionItem.Token.GetText()}";

    public SolutionIndexItem SolutionItem { get; set; }
    
    //public Bitmap? ImageType { get; } = ImageHelper.LoadFromResource(new Uri("avares://Assets/Icons/csharp-original.png"));
}