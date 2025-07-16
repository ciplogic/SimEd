using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using SimEd.Common.Interfaces;
using SimEd.Events;
using SimEd.Interfaces;
using SimEd.Models;
using SimEd.Views.Documents;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;

namespace SimEd.ViewModels.Documents;

public class FileViewModel : BaseFileViewModel, IViewAware
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

    public string Options
    {
        get => _options;
        set
        {
            if (SetProperty(ref _options, value))
            {
                ApplyEditorOptions();
            }
        }
    }

    private void ApplyEditorOptions()
    {
        MainControl.MainTextEditor.Options.ShowSpaces = true;
        MainControl.MainTextEditor.Options.ShowTabs = true;
        MainControl.MainTextEditor.Options.ShowEndOfLine = true;
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
        get => new(_settingsReader.Get().Font);
        set
        {
            if (_selectedFont == value) return;
            _settingsReader.Update(s => s.Font = value.Key + "#" + value.Name);
            SetProperty(ref _selectedFont, value);
        }
    }

    public void SetControl(Control control)
    {
        MainControl = (FileView)control;

        UpdateView();
    }

    private string _text = string.Empty;
    private string _encoding = string.Empty;
    private string _options = "[]";

    public void OnShowSpaces()
    {
        TextEditorOptions options = MainControl.MainTextEditor.Options;
        bool flagToToggle = options.ShowSpaces;
        flagToToggle = !flagToToggle;
        options.ShowSpaces = flagToToggle;
        options.ShowTabs = flagToToggle;
        options.ShowEndOfLine = flagToToggle;
        options.ShowEndOfLine = flagToToggle;
    }

    public void OnShowInExplorer()
    {
        DocumentUtilities.ShowInExplorer(Path);
    }

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
        IRawTheme? loadTheme = registryOptions.LoadTheme(ThemeName.Dark);
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

    private void ApplyThemeColorsToEditor(TextMate.Installation e)
    {
        TextEditor textEditor = MainControl.MainTextEditor!;
        ApplyBrushAction(e, "editor.background", brush => textEditor.Background = brush);
        ApplyBrushAction(e, "editor.foreground", brush => textEditor.Foreground = brush);

        if (!ApplyBrushAction(e, "editor.selectionBackground",
                brush => textEditor.TextArea.SelectionBrush = brush))
        {
            if (Application.Current!.TryGetResource("TextAreaSelectionBrush", out object? resourceObject))
            {
                if (resourceObject is IBrush brush)
                {
                    textEditor.TextArea.SelectionBrush = brush;
                }
            }
        }

        if (!ApplyBrushAction(e, "editor.lineHighlightBackground",
                brush =>
                {
                    textEditor.TextArea.TextView.CurrentLineBackground = brush;
                    textEditor.TextArea.TextView.CurrentLineBorder =
                        new Pen(brush); // Todo: VS Code didn't seem to have a border but it might be nice to have that option. For now just make it the same..
                }))
        {
            textEditor.TextArea.TextView.SetDefaultHighlightLineColors();
        }

        //Todo: looks like the margin doesn't have a active line highlight, would be a nice addition
        if (!ApplyBrushAction(e, "editorLineNumber.foreground",
                brush => textEditor.LineNumbersForeground = brush))
        {
            textEditor.LineNumbersForeground = textEditor.Foreground;
        }
    }

    private void ApplyThemeColorsToWindow(TextMate.Installation e)
    {
        Grid? statusBar = MainControl.MainStatusBar;

        if (!ApplyBrushAction(e, "statusBar.background", brush => statusBar.Background = brush))
        {
            statusBar.Background = Brushes.Purple;
        }

        if (!ApplyBrushAction(e, "statusBar.foreground", brush => MainControl.Foreground = brush))
        {
            MainControl.Foreground = Brushes.White;
        }

        //Applying the Editor background to the whole window for demo sake.
        ApplyBrushAction(e, "editor.background", brush => MainControl.MainTextEditor.Background = brush);
        ApplyBrushAction(e, "editor.foreground", brush => MainControl.MainTextEditor.Foreground = brush);
    }

    private bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
    {
        if (!e.TryGetThemeColor(colorKeyNameFromJson, out string? colorString))
            return false;

        if (!Color.TryParse(colorString, out Color color))
            return false;

        SolidColorBrush colorBrush = new SolidColorBrush(color);
        applyColorAction(colorBrush);
        return true;
    }
}