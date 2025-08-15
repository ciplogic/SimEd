using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using SimEd.ViewModels;

namespace SimEd.Models.FileChoosers;

internal class ClassicFileDialogChooser : IFileDialogChooser
{
    public async Task<string?> ChooseDirectory()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: { } window
            })
        {

            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Title = "Open Solution";
            string? result = await openFolderDialog.ShowAsync(window);
            return result;        
        }

        return null;
    }

    private static IStorageProvider StorageProvider => StorageService.GetStorageProvider()!;
}