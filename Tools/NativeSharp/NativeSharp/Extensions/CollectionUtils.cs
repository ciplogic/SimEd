using System.Reflection.Emit;
using NativeSharp.Common;

namespace NativeSharp.Extensions;

public static class CollectionUtils
{
    public static TOut[] SelectToArray<TIn, TOut>(this TIn[] source, Func<TIn, TOut> selector)
    {
        var result = new TOut[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }
        return result;
    }

    public static int[] BuildTargetBranches(this Instruction[] instructions2)
    {
        var targets = new HashSet<int>();
        foreach (var instruction in instructions2)
        {
            var opKind = instruction.OpCode.OperandType;
            switch (opKind)
            {
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    Instruction targetInstruction = (Instruction)instruction.Operand;
                    targets.Add(targetInstruction.Offset);
                    break;
            }
        }

        int[] targetBranches = targets.Order().ToArray();
        return targetBranches;
    }

}