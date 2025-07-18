namespace SimEd.IoC.Interfaces;

public interface IFileExtensionMapper
{
    string? MapExtension(string fullFilePath);
}