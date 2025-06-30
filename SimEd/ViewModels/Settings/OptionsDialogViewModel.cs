using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using SimEd.Common.Interfaces;
using SimEd.Events;

namespace SimEd.ViewModels.Settings;

public class OptionsDialogViewModel : ObservableObject
{
    private readonly IMiniPubSub _pubSub;
    
    private string _cascadiaFont = "avares://SimEd/Assets/Fonts/CascadiaCode-SemiBold.ttf#Cascadia Code";
    private string _lexendFont = "avares://SimEd/Assets/Fonts/Lexend-SemiBold.ttf#Lexend";
    private string _robotoMonoFont = "avares://SimEd/Assets/Fonts/RobotoMono-SemiBold.ttf#Roboto Mono";

    private ListBoxItem _selectedFont;

    public OptionsDialogViewModel(IMiniPubSub miniPubSub)
    {
        _pubSub = miniPubSub;
    }
    
    public string CascadiaFont
    {
        get => _cascadiaFont;
        private set => SetProperty(ref _cascadiaFont, value);
    }

    public string LexendFont
    {
        get => _lexendFont;
        private set => SetProperty(ref _lexendFont, value);
    }

    public string RobotoMonoFont
    {
        get => _robotoMonoFont;
        private set => SetProperty(ref _robotoMonoFont, value);
    }

    public ListBoxItem SelectedFont
    {
        get => _selectedFont;
        set
        {
            SetProperty(ref _selectedFont, value);
            PublishNewFont();
        } 
    }

    private void PublishNewFont()
    {
        var newFont = new OnChangeFontEvent(_selectedFont.FontFamily);
        _pubSub.Publish<OnChangeFontEvent>(newFont);
    }
}