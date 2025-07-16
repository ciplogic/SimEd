using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using SimEd.Interfaces;
using SimEd.Views.Documents;

namespace SimEd.ViewModels.Documents;

public class ImageFileViewModel : Document, IViewAware
{
    public void SetControl(Control control)
    {
        MainControl = (ImageFileView)control;
    }

    public ImageFileView MainControl { get; set; } = null!;
}