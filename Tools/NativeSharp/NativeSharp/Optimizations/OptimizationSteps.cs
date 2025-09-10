using NativeSharp.Operations.Common;
using NativeSharp.Optimizations.BlockOptimizations;
using NativeSharp.Optimizations.DeadCodeElimination;

namespace NativeSharp.Optimizations;

public class OptimizationSteps
{
    public CilMethodOptimization[] CilMethodOptimizations = BuildOptimizations();

    private static CilMethodOptimization[] BuildOptimizations()
    {
        return
        [
            new BlockBasedPropagation(),
            new RemovedUnusedAssignsAndVars()
        ];
    }

    public bool OptimizeMethod(CilNativeMethod method)
    {
        var result = false;
        foreach (var opt in CilMethodOptimizations)
        {
            result |= opt.Optimize(method);
        }

        return result;
    }
}