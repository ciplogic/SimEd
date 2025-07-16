using Avalonia.Controls;
using Avalonia.Input;
using SimEd.ViewModels.Search;

namespace SimEd.Views.Search;

public partial class ShowGenericFinderWindowView : Window
{
    ShowGenericFinderWindowViewModel ViewModel => (ShowGenericFinderWindowViewModel)DataContext!;

    public ShowGenericFinderWindowView()
    {
        InitializeComponent();
    }

    private void OnKeyDownHandler(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Down:
                ViewModel.SelectedIndex++;
                break;
            case Key.Up:
                ViewModel.SelectedIndex--;
                break;
            case Key.Enter:
                ViewModel.OnChosenItem();
                Close();
                break;
        }
    }

    private void OnDoubleTapped(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.OnChosenItem();
        Close();
        e.Handled = true;
    }
}