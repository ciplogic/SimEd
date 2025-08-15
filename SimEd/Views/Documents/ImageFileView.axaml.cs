using Avalonia.Controls;
using SimEd.ViewModels.Documents;

namespace SimEd.Views.Documents;

public partial class ImageFileView : UserControl
{
    private ImageFileViewModel ViewModel => (ImageFileViewModel)DataContext;

    public ImageFileView()
    {
        InitializeComponent();
    }
}