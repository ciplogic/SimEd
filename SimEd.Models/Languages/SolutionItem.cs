using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using U8;

namespace SimEd.Models.Languages;

public record SolutionItem(string Name, U8String Path, ObservableCollection<SolutionItem> Children, bool IsExpanded, SolutionItemMetadata Metadata)
{
    public SolutionItem AddChild(string name, U8String path, SolutionItemMetadata metadata)
    {
        SolutionItem child = new SolutionItem(name, path, [], true, metadata);
        Children.Add(child);
        return child;
    }
}