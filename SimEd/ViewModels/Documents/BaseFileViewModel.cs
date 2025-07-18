using Dock.Model.Mvvm.Controls;

namespace SimEd.ViewModels.Documents;

public class BaseFileViewModel : Document
{
    public string FullFilePath
    {
        get => _fullFilePath;
        set => SetProperty(ref _fullFilePath, value);
    }

    private string _fullFilePath = string.Empty;
}