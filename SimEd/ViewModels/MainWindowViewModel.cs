using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Core.Events;
using SimEd.Common.Interfaces;
using SimEd.Events;
using SimEd.Extensions;
using SimEd.Interfaces;
using SimEd.Models;
using SimEd.Models.Settings;
using SimEd.ViewModels.Documents;
using SimEd.ViewModels.Settings;
using SimEd.Views.Help;
using SimEd.Views.Settings;
using SimEd.ViewModels.Search;
using SimEd.Views.Help;
using SimEd.Views.Search;

namespace SimEd.ViewModels;

public class MainWindowViewModel : ObservableObject, IDropTarget
{
    private readonly IMiniPubSub _pubSub;
    private readonly IFactory? _factory;
    private IRootDock? _layout;

    public IInjector Provider { get; }

    public IRootDock? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }

    public MainWindowViewModel(IInjector serviceProvider, IMiniPubSub pubSub, IAppSettingsReader appSettingsReader)
    {
        _pubSub = pubSub;
        Provider = serviceProvider;
        _factory = serviceProvider.GetService<NotepadFactory>();

        Layout = _factory?.CreateLayout();
        if (Layout is { })
        {
            _factory?.InitLayout(Layout);
            _factory.ActiveDockableChanged += OnChangedFocus;
        }

        _pubSub.AddEventHandler<FileIsOpened>(OnFileOpened);
        AppSettings settings = appSettingsReader.Get();
        _pubSub.Publish(new ChangeSolutionFolderCommand(settings.Path));
    }


    public void CloseLayout()
    {
        if (Layout is not IDock dock)
        {
            return;
        }

        if (dock.Close.CanExecute(null))
        {
            dock.Close.Execute(null);
        }
    }

    public void FileNew()
    {
        FileViewModel untitledFileViewModel = GetUntitledFileViewModel();
        AddFileViewModel(untitledFileViewModel);
    }

    public async void FileOpen()
    {
        IStorageProvider? storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        IReadOnlyList<IStorageFile> results = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open layout",
            FileTypeFilter = GetOpenOpenLayoutFileTypes(),
            AllowMultiple = false
        });

        if (results is not { Count: > 0 })
        {
            return;
        }

        foreach (IStorageFile storageFile in results)
        {
            string path = storageFile.Path.AbsolutePath;
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            FileViewModel untitledFileViewModel = await OpenFileViewModel(path);
            AddFileViewModel(untitledFileViewModel);
        }
    }

    public void FileExit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Shutdown();
        }
    }

    public void HelpAbout()
    {
        HelpAboutWindow window = new();
        window.ShowDialog(GetWindow()!);
    }

    public void DragOver(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
    }

    public async void Drop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files))
        {
            return;
        }

        await HandleDropFiles(e);
        e.Handled = true;
    }


    private void OnChangedFocus(object? sender, ActiveDockableChangedEventArgs e)
    {
        FileViewModel? fileViewModel = e.Dockable as FileViewModel;
        if (fileViewModel is { })
        {
            _pubSub.Publish(new ChangedFocusedTab(fileViewModel));
        }
    }

    private async void OnFileOpened(FileIsOpened fileIsOpened)
    {
        FileViewModel? untitledFileViewModel = await OpenFileViewModel(fileIsOpened.FileItem.Path);
        if (untitledFileViewModel is null)
        {
            return;
        }

        AddFileViewModel(untitledFileViewModel);
    }

    private async Task<FileViewModel?> OpenFileViewModel(string path)
    {
        if (TryFindAlreadyOpenedTab(path, out FileViewModel fileViewModel))
        {
            return null;
        }

        if (!File.Exists(path))
        {
            return null;
        }

        Encoding encoding = FileTools.GetEncoding(path);
        string text = await File.ReadAllTextAsync(path, encoding).ConfigureAwait(false);
        string title = Path.GetFileName(path);
        FileViewModel openFileViewModel = Provider.GetService<FileViewModel>();
        openFileViewModel.Path = path;
        openFileViewModel.Title = title;
        openFileViewModel.Text = text;
        openFileViewModel.Encoding = encoding.WebName;
        return openFileViewModel;
    }

    private bool TryFindAlreadyOpenedTab(string path, out FileViewModel fileViewModel)
    {
        IDocumentDock? files = _factory?.GetDockable<IDocumentDock>("Files");
        fileViewModel = null!;
        if (files is null || files.VisibleDockables is null)
        {
            return false;
        }

        FileViewModel?[] allFiles =
            files.VisibleDockables.Select(x => x as FileViewModel).Where(x => x != null).ToArray();
        FileViewModel? foundFileViewModel = allFiles.FirstOrDefault(x => x.Path == path);
        if (foundFileViewModel is null)
        {
            return false;
        }

        files.ActiveDockable = foundFileViewModel;
        fileViewModel = foundFileViewModel;
        return true;
    }

    private static async Task SaveFileViewModel(FileViewModel fileViewModel)
    {
        await File.WriteAllTextAsync(fileViewModel.Path, fileViewModel.Text ?? "",
            Encoding.GetEncoding(fileViewModel.Encoding));
    }

    private void UpdateFileViewModel(FileViewModel fileViewModel, string path)
    {
        fileViewModel.Path = path;
        fileViewModel.Title = Path.GetFileName(path);
    }

    private void AddFileViewModel(FileViewModel fileViewModel)
    {
        IDocumentDock? files = _factory?.GetDockable<IDocumentDock>("Files");
        if (Layout is { } && files is { })
        {
            _factory?.AddDockable(files, fileViewModel);
            _factory?.SetActiveDockable(fileViewModel);
            _factory?.SetFocusedDockable(Layout, fileViewModel);
        }
    }

    private FileViewModel? GetFileViewModel()
    {
        IDocumentDock? files = _factory?.GetDockable<IDocumentDock>("Files");
        return files?.ActiveDockable as FileViewModel;
    }

    private FileViewModel GetUntitledFileViewModel()
    {
        FileViewModel untitledFileViewModel = Provider.GetService<FileViewModel>();
        untitledFileViewModel.Path = string.Empty;
        untitledFileViewModel.Title = "Untitled";
        untitledFileViewModel.Text = "";
        untitledFileViewModel.Encoding = Encoding.Default.WebName;
        return untitledFileViewModel;
    }

    private static List<FilePickerFileType> GetOpenOpenLayoutFileTypes() =>
    [
        StorageService.CSharp,
        StorageService.Java,
        StorageService.Json,
        StorageService.All
    ];

    public async void FileSave()
    {
        if (GetFileViewModel() is not { } fileViewModel)
        {
            return;
        }

        if (string.IsNullOrEmpty(fileViewModel.Path))
        {
            await FileSaveAsImpl(fileViewModel);
        }
        else
        {
            await SaveFileViewModel(fileViewModel);
        }
    }

    public async void FileSaveAs()
    {
        if (GetFileViewModel() is { } fileViewModel)
        {
            await FileSaveAsImpl(fileViewModel);
        }
    }

    public void OpenOptions()
    {
        var dataContext = Provider.GetService<OptionsDialogViewModel>();
        OptionsDialogView window = new()
        {
            DataContext = dataContext
        };
        window.Width = 350;
        window.Height = 200; 
        window.ShowDialog(GetWindow()!);
    }

    private async Task FileSaveAsImpl(FileViewModel fileViewModel)
    {
        SaveFileDialog dlg = new SaveFileDialog
        {
            Filters = new List<FileDialogFilter>
            {
                new() { Name = "Text document", Extensions = { "txt" } },
                new() { Name = "All", Extensions = { "*" } }
            },
            InitialFileName = fileViewModel.Title,
            DefaultExtension = "txt"
        };
        Window? window = GetWindow();
        if (window is null)
        {
            return;
        }

        string? result = await dlg.ShowAsync(window);
        if (result is { } && !string.IsNullOrEmpty(result))
        {
            UpdateFileViewModel(fileViewModel, result);
            await SaveFileViewModel(fileViewModel).ConfigureAwait(false);
        }
    }

    private async Task HandleDropFiles(DragEventArgs e)
    {
        IEnumerable<IStorageItem>? result = e.Data.GetFiles();
        if (result is null)
        {
            return;
        }

        foreach (IStorageItem storageItem in result)
        {
            string path = storageItem.Path.AbsolutePath;
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            FileViewModel? untitledFileViewModel = await OpenFileViewModel(path);
            AddFileViewModel(untitledFileViewModel);
        }
    }

    private static Window? GetWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            return desktopLifetime.MainWindow;
        }

        return null;
    }

    public async void OnShowGenericFinder()
    {
        _pubSub.Publish(new ShowGenericFinder(GetWindow()!));

        ShowGenericFinderWindowView window = new ShowGenericFinderWindowView()
        {
            DataContext = Provider.GetService<ShowGenericFinderWindowViewModel>()
        };
        await window.ShowDialog<object>(GetWindow());
    }
}