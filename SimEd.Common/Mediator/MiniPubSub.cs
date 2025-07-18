using SimEd.Common.Interfaces;

namespace SimEd.Common.Mediator;

public class MiniPubSub : IMiniPubSub
{
    private readonly TypedDictionaryOfValues<BaseHandler> _mapHandles = new();
    private readonly TypedDictionaryOfValues<BaseCommand> _mapCommands = new();
    private readonly TypedDictionaryOfValues<BaseQuery> _mapQuery = new();

    public void Publish<T>(T value)
    {
        HandlerList<T> tasksToExecute = GetHandlesOfType<T>();
        tasksToExecute.Publish(value);
    }

    public TR Query<T, TR>(T value)
    {
        Query<T, TR> tasksToExecute = GetQueryOfType<T, TR>();
        return tasksToExecute.Call(value);
    }

    public void Command<T>(T value)
    {
        Command<T> tasksToExecute = GetCommandOfType<T>();
        tasksToExecute.Call(value);
    }

    public IMiniPubSub AddEventHandler<T>(Action<T> handler)
    {
        GetHandlesOfType<T>().Add(handler);
        return this;
    }

    public IMiniPubSub AddCommandHandler<T>(Action<T> handler)
    {
        GetCommandOfType<T>().Set(handler);
        return this;
    }

    public IMiniPubSub AddQueryHandler<T, TR>(Func<T, TR> handler)
    {
        GetQueryOfType<T, TR>().Set(handler);
        return this;
    }

    public HandlerList<T> GetHandlesOfType<T>()
        => (HandlerList<T>)_mapHandles.UpdatedValue<T>(() => new HandlerList<T>());

    public Command<T> GetCommandOfType<T>()
        => (Command<T>)_mapCommands.UpdatedValue<T>(() => new Command<T>());

    public Query<T, TR> GetQueryOfType<T, TR>()
        => (Query<T, TR>)_mapQuery.UpdatedValue<T>(() => new Query<T, TR>());

    public void RemoveEventHandler<T>(Action<T> onZoomChanged) 
        => GetHandlesOfType<T>().Remove(onZoomChanged);
}