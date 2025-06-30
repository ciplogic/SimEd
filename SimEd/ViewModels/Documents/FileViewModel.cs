using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit.TextMate;
using Dock.Model.Mvvm.Controls;
using SimEd.Common.Interfaces;
using SimEd.Events;
using SimEd.Interfaces;
using SimEd.Models;
using SimEd.Models.Settings;
using SimEd.Views.Documents;
using TextMateSharp.Grammars;

namespace SimEd.ViewModels.Documents;

public class FileViewModel : Document, IViewAware
{
    private readonly IMiniPubSub _pubSub;
    private readonly IAppSettingsReader _settingsReader;

    private FontFamily _selectedFont;

    public FileViewModel(IMiniPubSub pubSub, IAppSettingsReader settingsReader)
    {
        _pubSub = pubSub;
        _settingsReader = settingsReader;
        _pubSub.AddEventHandler<ZoomFontLevelChanged>(OnZoomChanged);
        _pubSub.AddEventHandler<OnChangeFontEvent>(OnFontFamilyChange);
    }

    public override bool OnClose()
    {
        _pubSub.RemoveEventHandler<ZoomFontLevelChanged>(OnZoomChanged);
        return base.OnClose();
    }

    private void OnZoomChanged(ZoomFontLevelChanged zoomFontLevel)
        => FontSize = zoomFontLevel.FontSize;

    public string Path
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }

    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public string Encoding
    {
        get => _encoding;
        set => SetProperty(ref _encoding, value);
    }

    public FileView MainControl { get; set; }

    public int FontSize
    {
        get => _settingsReader.Get().FontSize;
        set
        {
            if (value == FontSize) return;
            _settingsReader.Update(s => s.FontSize = value);
            OnPropertyChanged();
        }
    }

    public Avalonia.Media.FontFamily SelectedFont
    {
        get => new (_settingsReader.Get().Font);
        set
        {
            if (_selectedFont == value) return;
            _settingsReader.Update(s => s.Font = value.Key + "#" +value.Name);
            SetProperty(ref _selectedFont, value);   
        }
    }

    public void SetControl(Control control)
    {
        MainControl = (FileView)control;

        UpdateView();
    }

    private string _path = string.Empty;
    private string _text = string.Empty;
    private string _encoding = string.Empty;

    private void UpdateView()
    {
        RegistryOptions registryOptions = new RegistryOptions(ThemeName.Light);
        TextMate.Installation? textMateInstallation = MainControl.MainTextEditor.InstallTextMate(registryOptions);
        string extension = ExtensionOfFile(Path);
        if (string.IsNullOrEmpty(extension))
        {
            extension = ".txt";
        }

        Language csharpLanguage = registryOptions.GetLanguageByExtension(extension);
        string scopeName = registryOptions.GetScopeByLanguageId(csharpLanguage?.Id ?? "");
        textMateInstallation.SetGrammar(scopeName);
    }

    private static string ExtensionOfFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        FileInfo fileInfo = new(fileName);
        return fileInfo.Exists
            ? fileInfo.Extension
            : string.Empty;
    }

    public void PushUpdateSettings(int deltaY)
    {
        _settingsReader.Update(appSettings =>
        {
            int fontSize = appSettings.FontSize;

            fontSize = deltaY > 0 
                ? fontSize + 1 
                : fontSize > 1 
                    ? fontSize - 1 
                    : 1;

            appSettings.FontSize = fontSize;

            _pubSub.Publish<ZoomFontLevelChanged>(new(fontSize));
        });
    }

    private void OnFontFamilyChange(OnChangeFontEvent fontEvent)
        => SelectedFont = fontEvent.SelectedFont;
}