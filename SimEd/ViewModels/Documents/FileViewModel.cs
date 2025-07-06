using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit.TextMate;
using Dock.Model.Mvvm.Controls;
using SimEd.Common.Interfaces;
using SimEd.Events;
using SimEd.Interfaces;
using SimEd.Models;
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

    public FontFamily SelectedFont
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
        RegistryOptions registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        TextMate.Installation textMateInstallation = MainControl.MainTextEditor.InstallTextMate(registryOptions);

        textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;
        string extension = ExtensionOfFile(Path);
        if (string.IsNullOrEmpty(extension))
        {
            extension = ".txt";
        }

        Language csharpLanguage = registryOptions.GetLanguageByExtension(extension);
        string scopeName = registryOptions.GetScopeByLanguageId(csharpLanguage?.Id ?? "");
        var loadTheme = registryOptions.LoadTheme(ThemeName.Dark);
        MainControl.MainTextEditor.Options.HighlightCurrentLine = true;
        textMateInstallation.SetTheme(loadTheme);
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
    
    
    private void TextMateInstallationOnAppliedTheme(object sender, TextMate.Installation e)
    {
        ApplyThemeColorsToEditor(e);
        ApplyThemeColorsToWindow(e);
    }

    void ApplyThemeColorsToEditor(TextMate.Installation e)
    {
        var _textEditor = MainControl.MainTextEditor;
        ApplyBrushAction(e, "editor.background",brush => _textEditor.Background = brush);
        ApplyBrushAction(e, "editor.foreground",brush => _textEditor.Foreground = brush);

        if (!ApplyBrushAction(e, "editor.selectionBackground",
                brush => _textEditor.TextArea.SelectionBrush = brush))
        {
            if (Application.Current!.TryGetResource("TextAreaSelectionBrush", out var resourceObject))
            {
                if (resourceObject is IBrush brush)
                {
                    _textEditor.TextArea.SelectionBrush = brush;
                }
            }
        }

        if (!ApplyBrushAction(e, "editor.lineHighlightBackground",
                brush =>
                {
                    _textEditor.TextArea.TextView.CurrentLineBackground = brush;
                    _textEditor.TextArea.TextView.CurrentLineBorder = new Pen(brush); // Todo: VS Code didn't seem to have a border but it might be nice to have that option. For now just make it the same..
                }))
        {
            _textEditor.TextArea.TextView.SetDefaultHighlightLineColors();
        }

        //Todo: looks like the margin doesn't have a active line highlight, would be a nice addition
        if (!ApplyBrushAction(e, "editorLineNumber.foreground",
                brush => _textEditor.LineNumbersForeground = brush))
        {
            _textEditor.LineNumbersForeground = _textEditor.Foreground;
        }
    }

    private void ApplyThemeColorsToWindow(TextMate.Installation e)
    {
        var statusBar = MainControl.MainStatusBar;

        if (!ApplyBrushAction(e, "statusBar.background", brush => statusBar.Background = brush))
        {
            statusBar.Background = Brushes.Purple;
        }

        if (!ApplyBrushAction(e, "statusBar.foreground", brush => MainControl.Foreground = brush))
        {
            MainControl.Foreground = Brushes.White;
        }

        //Applying the Editor background to the whole window for demo sake.
        ApplyBrushAction(e, "editor.background",brush => MainControl.MainTextEditor.Background = brush);
        ApplyBrushAction(e, "editor.foreground",brush => MainControl.MainTextEditor.Foreground = brush);
    }

    bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
    {
        if (!e.TryGetThemeColor(colorKeyNameFromJson, out var colorString))
            return false;

        if (!Color.TryParse(colorString, out Color color))
            return false;

        var colorBrush = new SolidColorBrush(color);
        applyColorAction(colorBrush);
        return true;
    }
}