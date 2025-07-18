using SimEd.IoC.Interfaces;

namespace SimEd.ViewModels.Documents;

public class FileExtensionMapper : IFileExtensionMapper
{
    public string? MapExtension(string fullFilePath)
    {
        var fileInfo = new FileInfo(fullFilePath);
        var fileName = fileInfo.Name;
        var fileExtension = fileInfo.Extension;
        switch (fileExtension)
        {
            case ".bzl":
                return ".py";
        }

        return fileName switch
        {
            "WORKSPACE" or "BUILD" => ".py",
            _ => null
        };
    }
}