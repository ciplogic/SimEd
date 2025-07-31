using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SimEd.ViewModels.Documents;

namespace SimEd.Views.Documents;

public partial class FileView : UserControl
{
    private FileViewModel ViewModel => (FileViewModel)DataContext! ;

    public FileView()
    {
        InitializeComponent();

        AddHandler(PointerWheelChangedEvent, (o, i) =>
        {
            if (i.KeyModifiers == KeyModifiers.Control)
            {
                ViewModel.PushUpdateSettings((int)i.Delta.Y);
            }
        }, RoutingStrategies.Bubble, true);
    }

    private void MainTextEditor_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        ViewModel.SaveToFile();
    }
}