using Avalonia.Platform;
using Avalonia.Svg.Skia;

namespace SimEd.Models.Languages;

public sealed record SolutionItemMetadata
{
    public string Kind { get; private set; }
    public SvgImage IconSource { get; }

    private static readonly string[] _sourceArray = new[] {"props","manifest","userSettings","settings","config","user"};
    
    public SolutionItemMetadata(string kind)
    {
        Kind = kind;
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

    private static SvgImage CreateSvgSource(string path)
    {
        var svg = new SvgImage
        {
            Source = SvgSource.LoadFromStream(AssetLoader.Open(new Uri(path)))
        };
        return svg;
    }
}