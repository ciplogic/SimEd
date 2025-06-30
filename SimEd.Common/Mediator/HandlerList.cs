using SimEd.Common.Extensions;

namespace SimEd.Common.Mediator;

public class HandlerList<T> : BaseHandler
{
    public Action<T>[] Handlers { get; private set; } = [];

    public void Clear()
        => Handlers = [];

    public void Add(Action<T> handler)
        => Handlers = Handlers.ConcatArray(handler);

    public void Publish(T arg)
    {
        foreach (Action<T> handler in Handlers)
        {
            handler(arg);
        }
    }

    public void Remove(Action<T> action) 
        => Handlers = Handlers
            .Where(handler => ReferenceEquals(handler, action))
            .ToArray();
}
