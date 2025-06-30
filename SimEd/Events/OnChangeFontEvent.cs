using Avalonia.Media;

namespace SimEd.Events;

public readonly struct OnChangeFontEvent(FontFamily selectedFont)
{
    public FontFamily SelectedFont { get; } = selectedFont;
}