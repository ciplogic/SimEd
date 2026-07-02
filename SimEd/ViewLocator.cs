using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using SimEd.Interfaces;
using StaticViewLocator;

namespace SimEd;

[StaticViewLocator]
public partial class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        Type type = data.GetType();

        if (!s_views.TryGetValue(type, out Func<Control>? func))
        {
            throw new Exception($"Unable to create view for type: {type}");
        }

        Control resultControl = func.Invoke();
        resultControl.DataContext = data;
        if (data is IViewAware viewAware)
        {
            viewAware.SetControl(resultControl);
        }

        return resultControl;
    }

    public bool Match(object? data)
        => data is ObservableObject or IDockable;
}