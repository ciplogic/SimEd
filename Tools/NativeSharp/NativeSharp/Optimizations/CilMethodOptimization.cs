using NativeSharp.Operations.Common;

namespace NativeSharp.Optimizations;

public abstract class CilMethodOptimization
{
    public abstract bool Optimize(CilNativeMethod cilNativeMethod);
}