using System.Collections.ObjectModel;

namespace SimEd.Models.Languages;

public record SolutionItem(string Name, string Path, ObservableCollection<SolutionItem> Children, bool IsExpanded, string Kind)
{
    public SolutionItem AddChild(string name, string path, string kind)
    {
        SolutionItem child = new SolutionItem(name, path, [], true, kind);
        Children.Add(child);
        return child;
    }
}