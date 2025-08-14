using System.Text.RegularExpressions;
using ZLinq;

namespace SimEd.ViewModels.Solution;

public class GitIgnoreScanner
{
    public Func<string, bool>[] IgnoredFiles { get; set; } = [];

    string TargetDirectory { get; set; } = string.Empty;

    public void ScanDirectory(DirectoryInfo directoryInfo)
    {
        Clear();

        TargetDirectory = GetTargetDirectoryAsUnixPath(directoryInfo);

        string gitIgnoreFile = Path.Combine(directoryInfo.FullName, ".gitignore");
        if (!File.Exists(gitIgnoreFile))
        {
            return;
        }

        string[] lines = File.ReadAllLines(gitIgnoreFile);
        string[] goodFiles = lines
            .AsValueEnumerable()
            .Select(l => l.Trim())
            .Where(x => x.Length > 0 && x[0] != '#')
            .ToArray();
        IgnoredFiles = BuildFilters(goodFiles);
    }

    private static string GetTargetDirectoryAsUnixPath(DirectoryInfo directoryInfo)
    {
        string targetDirectory = directoryInfo.FullName.Replace('\\', '/');

        if (targetDirectory[^1] == '/')
        {
            targetDirectory = targetDirectory[..^1];
        }

        return targetDirectory;
    }

    private static Func<string, bool>[] BuildFilters(string[] goodFiles)
    {
        List<Func<string, bool>> filters =
        [
            x =>
            {
                string gitFolder = $"{Path.DirectorySeparatorChar}.git";
                return x.EndsWith(gitFolder);
            },
            x =>
            {
                string gitFolder = $"{Path.DirectorySeparatorChar}.gitignore";
                return x.EndsWith(gitFolder);
            }
        ];

        foreach (string file in goodFiles)
        {
            filters.Add(MapGitIgnoreEntry(file));
        }

        return filters.ToArray();
    }

    string FormatFileNameToUnix(string fullFileName)
    {
        var fullFileInfo = new FileInfo(fullFileName);
        var replacedName = fullFileInfo.FullName.Replace('\\', '/');
        var reducedName = replacedName.Substring(TargetDirectory.Length);
        return reducedName;
    }

    static Func<string, bool> MapGitIgnoreEntry(string gitIgnoreFileFilter)
    {
        var globFilter = new GlobFilter(gitIgnoreFileFilter);
        return globFilter.Matches;
        var containsSlash = gitIgnoreFileFilter.IndexOf('/') != -1;
        var lastSlashIndex = gitIgnoreFileFilter.LastIndexOf('/');

        var containsStar = gitIgnoreFileFilter.IndexOf('*') != -1;
        if (!containsSlash && !containsStar)
        {
            return fileName =>
            {
                var fileInfo = new FileInfo(fileName);
                return fileInfo.Name == gitIgnoreFileFilter;
            };
        }

        return fileName => fileName.Contains(gitIgnoreFileFilter);
    }

    private void Clear()
    {
        IgnoredFiles = [];
    }

    public bool IgnorePath(string directoryFullName)
    {
        var unixFileName = FormatFileNameToUnix(directoryFullName);
        return IgnoredFiles
            .AsValueEnumerable()
            .Any(ignoredFileFilter => ignoredFileFilter(unixFileName));
    }
}