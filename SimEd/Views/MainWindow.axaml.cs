using Avalonia.Controls;
using Avalonia.Input;
using SimEd.ViewModels;

namespace SimEd.Views;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel 
        => (MainWindowViewModel)DataContext;

    public MainWindow()
        => InitializeComponent();

    private void OnKeyDownHandler(object? sender, KeyEventArgs e)
    {
        if (e is { Key: Key.T, KeyModifiers: KeyModifiers.Control })
        {
            ViewModel.OnShowGenericFinder();
            e.Handled = true;
        }
    }
}
