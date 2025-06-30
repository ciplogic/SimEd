namespace SimEd.Common.Extensions;

public static class CollectionExtensions
{
    public static T[] ConcatArray<T>(this T[] items, T added)
        => items.ConcatArrays([added]);

    public static T[] ConcatArrays<T>(this T[] items, T[] added)
    {
        List<T> result = new List<T>(items.Length + added.Length);
        result.AddRange(items);
        result.AddRange(added);
        return result.ToArray();
    }

    public static TDest[] SelectToArray<T, TDest>(this T[] items, Func<T, TDest> selector)
        => items.Length == 0
            ? []
            : Array.ConvertAll(items, x => selector(x));

    public static TDest[] SelectToArray<T, TDest>(this IList<T> items, Func<T, TDest> selector)
    {
        if (items.Count == 0)
        {
            return [];
        }

        TDest[] result = new TDest[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            result[i] = selector(items[i]);
        }
        return result;
    }
}
