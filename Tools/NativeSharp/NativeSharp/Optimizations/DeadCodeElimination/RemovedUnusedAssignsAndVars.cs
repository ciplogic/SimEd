using NativeSharp.Operations;
using NativeSharp.Operations.Common;

namespace NativeSharp.Optimizations.DeadCodeElimination;

public class RemovedUnusedAssignsAndVars : CilMethodOptimization
{
    public override bool Optimize(CilNativeMethod cilNativeMethod)
    {
        var candidatesForRemoval = new HashSet<string>();

        foreach (BaseOp instruction in cilNativeMethod.Instructions)
        {
            if (instruction is AssignOp assignOp)
            {
                candidatesForRemoval.Add(assignOp.Left.Code());
            }
        }

        foreach (BaseOp instruction in cilNativeMethod.Instructions)
        {
            RemoveUsages(instruction, candidatesForRemoval);
            if (candidatesForRemoval.Count == 0)
            {
                return false;
            }
        }

        var indicesToRemove = new List<int>();
        for (var index = 0; index < cilNativeMethod.Instructions.Length; index++)
        {
            var instruction = cilNativeMethod.Instructions[index];
            if (instruction is AssignOp assignOp)
            {
                if (candidatesForRemoval.Contains(assignOp.Left.Code()))
                {
                    indicesToRemove.Add(index);
                }
            }
        }
        
        CilMethodExtensions.RemoveIndices(cilNativeMethod, indicesToRemove.ToArray());
        var vars = cilNativeMethod.Locals
            .Where(localVar => !candidatesForRemoval.Contains(localVar.Code()))
            .ToArray();
        cilNativeMethod.Locals = vars;

        return candidatesForRemoval.Count != 0;
    }

    private void RemoveUsages(BaseOp instruction, HashSet<string> candidatesForRemoval)
    {
        IEnumerable<string> usagesOfOp = InstructionUsages.GetUsagesOf(instruction);
        foreach (string usage in usagesOfOp)
        {
            candidatesForRemoval.Remove(usage);
        }
    }

    
}