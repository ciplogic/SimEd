using NativeSharp.Operations;

namespace NativeSharp.Extensions;

public static class BaseOpsExtensions
{
    public static int IndexOfOp<TOp>(this BaseOp[] ops, Predicate<TOp> predicate) where TOp : BaseOp
    {
        for (int i = 0; i < ops.Length; i++)
        {
            if (ops[i] is not TOp op)
            {
                continue;
            }

            if (predicate(op))
            {
                return i;
            }
        }

        return -1;
    }

    public static int[] IndexesOfOp<TOp>(this BaseOp[] ops, Predicate<TOp> predicate) where TOp : BaseOp
    {
        List<int> result = [];
        for (int i = 0; i < ops.Length; i++)
        {
            if (ops[i] is not TOp op)
            {
                continue;
            }

            if (predicate(op))
            {
                result.Add(i);
            }
        }

        return result.ToArray();
    }
}