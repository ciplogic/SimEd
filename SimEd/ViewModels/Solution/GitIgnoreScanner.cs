using System.Text.RegularExpressions;
using ZLinq;

namespace SimEd.ViewModels.Solution;

public class GitIgnoreScanner
{
    public Func<string, bool>[] IgnoredFiles { get; set; } = [];
    public string[] IgnoredFilters { get; set; } = [];

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
        string[] gitIgnoreRules = lines
            .AsValueEnumerable()
            .Select(l => l.Trim())
            .Where(x => x.Length > 0 && x[0] != '#')
            .ToArray();
        IgnoredFilters = gitIgnoreRules;
        IgnoredFiles = BuildFilters(gitIgnoreRules);
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
        var reducedName = replacedName[TargetDirectory.Length..];
        return reducedName;
    }

    private static Func<string, bool> MapGitIgnoreEntry(string gitIgnoreFileFilter)
        => new GlobFilter(gitIgnoreFileFilter).Matches;

    private void Clear()
    {
        IgnoredFiles = [];
    }

    public bool IgnoreFileIfFiltered(string directoryFullName)
    {
        var unixFileName = FormatFileNameToUnix(directoryFullName);
        bool result = false;
        foreach (var ignoredFileFilter in IgnoredFiles)
        {
            if (ignoredFileFilter(unixFileName))
            {
                result = true;
                break;
            }
        }

        if (result)
        {
            Console.WriteLine($"Ignored file: {unixFileName}");
        }

        return result;
    }
}