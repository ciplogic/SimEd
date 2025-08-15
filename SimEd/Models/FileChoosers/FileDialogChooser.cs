using Avalonia.Platform.Storage;
using SimEd.ViewModels;

namespace SimEd.Models.FileChoosers;

internal class FileDialogChooser : IFileDialogChooser
{
    public async Task<string?> ChooseDirectory()
    {
        IStorageFolder? suggestedLocation = await StorageProvider.TryGetFolderFromPathAsync(Directory.GetCurrentDirectory());
        IReadOnlyList<IStorageFolder> results = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions()
            {
                Title = "Open Solution",
                AllowMultiple = false,
                SuggestedFileName = "Folder",
                SuggestedStartLocation = suggestedLocation
            });

        if (results is { Count: > 0 })
        {
            Uri selectedDir = results[0].Path;
            return new DirectoryInfo(selectedDir.AbsolutePath).FullName;
        }

        return null;
    }

    private static IStorageProvider StorageProvider => StorageService.GetStorageProvider()!;
}