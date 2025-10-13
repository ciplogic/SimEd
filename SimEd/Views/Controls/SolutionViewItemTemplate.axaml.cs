using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Svg.Skia;

namespace SimEd.Views.Controls;

public class SolutionViewItemTemplate : TemplatedControl
{
    public static readonly StyledProperty<SvgImage> SourceProperty =
        AvaloniaProperty.Register<SolutionViewItemTemplate, SvgImage>(nameof(Source), new SvgImage());
    
    public static readonly StyledProperty<string> NameProperty =
        AvaloniaProperty.Register<SolutionViewItemTemplate, string>(nameof(Name), string.Empty);

    public SvgImage Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    public string Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }
}