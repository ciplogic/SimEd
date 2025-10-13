using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using U8;

namespace SimEd.Models.Languages;

public record SolutionItem(string Name, U8String Path, ObservableCollection<SolutionItem> Children, bool IsExpanded, string Kind, SolutionItemMetadata Metadata)
{
    public SolutionItem AddChild(string name, U8String path, string kind)
    {
        SolutionItem child = new SolutionItem(name, path, [], true, kind, new SolutionItemMetadata(kind));
        Children.Add(child);
        return child;
    }
}

public sealed record SolutionItemMetadata
{
    public SvgImage IconSource { get; }

    private static readonly string[] _sourceArray = new[] {"props","manifest","userSettings","settings","config","user"};

    public SolutionItemMetadata(string kind)
    {
        IconSource = GetIconByKind(kind);
    }

    private SvgImage GetIconByKind(string kind)
        => kind switch
        {
            "" => CreateSvgSource("avares://SimEd/Assets/Icons/folder.svg"),
            "cs" => CreateSvgSource("avares://SimEd/Assets/Icons/csharp-line.svg"),
            "csproj" => CreateSvgSource("avares://SimEd/Assets/Icons/csproj.svg"),
            "dll" => CreateSvgSource("avares://SimEd/Assets/Icons/dll.svg"),
            "pdb" => CreateSvgSource("avares://SimEd/Assets/Icons/pdb.svg"),
            "py" => CreateSvgSource("avares://SimEd/Assets/Icons/python.svg"),
            "sln" => CreateSvgSource("avares://SimEd/Assets/Icons/sln-svgrepo-com.svg"),
            "json" => CreateSvgSource("avares://SimEd/Assets/Icons/brackets-svgrepo-com.svg"),
            "md" => CreateSvgSource("avares://SimEd/Assets/Icons/markdown-svgrepo-com.svg"),
            "java" => CreateSvgSource("avares://SimEd/Assets/Icons/java-plain.svg"),
            not null when _sourceArray.Contains(kind) => CreateSvgSource("avares://SimEd/Assets/Icons/settings-svgrepo-com.svg"),
            _ => CreateSvgSource("avares://SimEd/Assets/Icons/square.svg")
        };

    private SvgImage CreateSvgSource(string path)
    {
        var svg = new SvgImage
        {
            Source = SvgSource.LoadFromStream(AssetLoader.Open(new Uri(path)))
        };
        return svg;
    }
}