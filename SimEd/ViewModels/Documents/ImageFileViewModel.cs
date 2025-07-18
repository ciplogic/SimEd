using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using SimEd.Interfaces;
using SimEd.Views.Documents;

namespace SimEd.ViewModels.Documents;

public class ImageFileViewModel : BaseFileViewModel, IViewAware
{
    private Bitmap? _imageFromPicture;
    private double _width;
    private double _height;


    public void SetControl(Control control)
    {
        MainControl = (ImageFileView)control;
    }

    public ImageFileView MainControl { get; set; } = null!;

    public static string[] SupportedFormats { get; } = [".png", ".jpg", ".bmp", ".webp"];

    public Bitmap? ImageFromPicture
    {
        get => _imageFromPicture;
        set => SetProperty(ref _imageFromPicture, value);
    }

    public double Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    public double Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(FullFilePath))
        {
            if (!File.Exists(FullFilePath))
            {
                return;
            }

            ImageFromPicture = new Bitmap(FullFilePath);
            Width = ImageFromPicture.Size.Width;
            Height = ImageFromPicture.Size.Height;
        }
    }

    public void OnOpen()
    {
        FileInfo fileInfo = new (FullFilePath);
        Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = true,
            Verb = "open",
            FileName = fileInfo.FullName,
            WorkingDirectory = fileInfo.DirectoryName
        });
    }
}