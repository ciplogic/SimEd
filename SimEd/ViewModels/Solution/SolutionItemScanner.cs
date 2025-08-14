using SimEd.IoC.Interfaces;
using SimEd.Models.Languages;
using ZLinq;

namespace SimEd.ViewModels.Solution;

internal static class SolutionItemScanner
{
    public static SolutionItem ScanDirectory(DirectoryInfo parentDirInfo, DirectoryInfo dirInfo, GitIgnoreScanner gitIgnoreScanner,
        IFileExtensionMapper fileExtensionMapper)
    {
        DirectoryInfo[] directoryInfos = dirInfo.GetDirectories();
        FileInfo[] fileInfos = dirInfo.GetFiles();
        SolutionItem result = new SolutionItem(dirInfo.Name, dirInfo.FullName, [], false, string.Empty);

        foreach (DirectoryInfo directory in directoryInfos)
        {
            if (gitIgnoreScanner.IgnorePath(directory.FullName))
            {
                continue;
            }

            SolutionItem child = ScanDirectory(parentDirInfo, directory, gitIgnoreScanner, fileExtensionMapper);
            result.Children.Add(child);
        }

        foreach (FileInfo file in fileInfos)
        {
            if (gitIgnoreScanner.IgnorePath(file.FullName))
            {
                continue;
            }

            string extension = fileExtensionMapper.MapExtension(file.FullName) ?? file.Extension;
            extension = extension.Replace(".", string.Empty);

            result.AddChild(file.Name, file.FullName, extension);
        }

        return result;
    }
}