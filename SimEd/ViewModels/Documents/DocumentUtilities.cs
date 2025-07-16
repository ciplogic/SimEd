using System.Diagnostics;

namespace SimEd.ViewModels.Documents;

public static class DocumentUtilities
{
    public static void ShowInExplorer(string filePath)
    {
        var osId = Environment.OSVersion.Platform;
        switch (osId)
        {
            case PlatformID.Win32NT:
                ShowExplorerInWindows(filePath);
                break;
            case PlatformID.MacOSX:
                ShowExplorerInMacOs(filePath);
                break;
        }
    }

    private static void ShowExplorerInMacOs(string filePath)
    {
        var directoryInfo = new FileInfo(filePath).Directory;
        Process.Start("finder", directoryInfo!.FullName);
    }

    private static void ShowExplorerInWindows(string filePath)
    {
        string argument = "/select, \"" + filePath + "\"";
        Process.Start("explorer.exe", argument);
    }
}