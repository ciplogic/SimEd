using Dock.Model.Mvvm.Controls;

namespace SimEd.ViewModels.Documents;

public class BaseFileViewModel : Document
{
    public string Path
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }

    private string _path = string.Empty;
}