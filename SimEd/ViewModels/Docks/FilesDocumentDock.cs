using System.Text;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using SimEd.Common.Interfaces;
using SimEd.ViewModels.Documents;

namespace SimEd.ViewModels.Docks;

public class FilesDocumentDock : DocumentDock
{
    private readonly IInjector _injector;

    public FilesDocumentDock(IInjector injector)
    {
        _injector = injector;
        CreateDocument = new RelayCommand(CreateNewDocument);
    }

    private void CreateNewDocument()
    {
        if (!CanCreateDocument)
        {
            return;
        }

        FileViewModel document = _injector.GetService<FileViewModel>();
        document.Path = string.Empty;
        document.Title = "Untitled";
        document.Text = "";
        document.Encoding = Encoding.UTF8.WebName;

        Factory?.AddDockable(this, document);
        Factory?.SetActiveDockable(document);
        Factory?.SetFocusedDockable(this, document);
    }
}