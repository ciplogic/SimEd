using Dock.Model.Mvvm.Controls;
using U8;

namespace SimEd.ViewModels.Documents;

public class BaseFileViewModel : Document
{
    public string FullFilePath
    {
        get => _fullFilePath.ToString();
        set => SetProperty(ref _fullFilePath, value.ToU8String());
    }

    private U8String _fullFilePath = U8String.Empty;
}