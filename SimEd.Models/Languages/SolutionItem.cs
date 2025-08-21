using System.Collections.ObjectModel;
using U8;

namespace SimEd.Models.Languages;

public record SolutionItem(string Name, U8String Path, ObservableCollection<SolutionItem> Children, bool IsExpanded, string Kind)
{
    public SolutionItem AddChild(string name, U8String path, string kind)
    {
        SolutionItem child = new SolutionItem(name, path, [], true, kind);
        Children.Add(child);
        return child;
    }
}