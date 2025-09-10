using NativeSharp.Operations;
using NativeSharp.Operations.Common;

namespace NativeSharp.Optimizations.DeadCodeElimination;

public static class CilMethodExtensions
{
    public static void RemoveIndices(CilNativeMethod cilNativeMethod, int[] indicesToRemove)
    {
        var newOps = new List<BaseOp>();
        for (var index = 0; index < cilNativeMethod.Instructions.Length; index++)
        {
            if (indicesToRemove.Contains(index))
            {
                continue;
            }
            var op = cilNativeMethod.Instructions[index];
            newOps.Add(op);
        }
        cilNativeMethod.Instructions = newOps.ToArray();
    }
}