using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using SimEd.Common.Interfaces;
using SimEd.ViewModels.Docks;
using SimEd.ViewModels.Solution;

namespace SimEd.ViewModels;

public class NotepadFactory(IInjector injector) : Factory
{
    private IRootDock? _rootDock;
    private IDocumentDock? _documentDock;

    public override IDocumentDock CreateDocumentDock() => new FilesDocumentDock(injector);

    public override IRootDock CreateLayout()
    {
        SolutionViewModel solutionViewModel = injector.GetService<SolutionViewModel>();
        solutionViewModel.Id = "Solution";
        solutionViewModel.Title = "Solution";
        
        FilesDocumentDock documentDock = new(injector)
        {
            Id = "Files",
            Title = "Files",
            IsCollapsable = false,
            Proportion = double.NaN,
            VisibleDockables = CreateList<IDockable>
            (
            ),
            CanCreateDocument = true
        };
        ProportionalDock solutionDock = new()
        {
            Proportion = 0.2,
            Orientation = Orientation.Vertical,
            CanClose = false,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = solutionViewModel,
                    VisibleDockables = CreateList<IDockable>
                    (
                        solutionViewModel
                    ),
                    CanClose = false,
                    Alignment = Alignment.Left,
                    GripMode = GripMode.Visible
                }
            )
        };

        IRootDock windowLayout = CreateRootDock();
        windowLayout.Title = "Default";
        ProportionalDock windowLayoutContent = new()
        {
            Orientation = Orientation.Horizontal,
            IsCollapsable = false,
            VisibleDockables = CreateList<IDockable>
            (
                solutionDock,
                new ProportionalDockSplitter(),
                documentDock
            )
        };
        windowLayout.IsCollapsable = false;
        windowLayout.VisibleDockables = CreateList<IDockable>(windowLayoutContent);
        windowLayout.ActiveDockable = windowLayoutContent;

        IRootDock rootDock = CreateRootDock();

        rootDock.IsCollapsable = false;
        rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
        rootDock.ActiveDockable = windowLayout;
        rootDock.DefaultDockable = windowLayout;

        _documentDock = documentDock;
        _rootDock = rootDock;
        return rootDock;
    }

    private void OnNavigate()
    {
        
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new()
        {
            ["Find"] = () => layout,
            ["Replace"] = () => layout
        };

        DockableLocator = new Dictionary<string, Func<IDockable?>>
        {
            ["Root"] = () => _rootDock,
            ["Files"] = () => _documentDock
        };

        HostWindowLocator = new()
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }
}