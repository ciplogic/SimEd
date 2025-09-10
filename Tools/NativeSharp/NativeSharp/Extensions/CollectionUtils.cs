using System.Reflection;
using System.Reflection.Emit;
using NativeSharp.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Extensions;

public static class CollectionUtils
{

    public static int[] BuildTargetBranches(this Instruction[] instructions2)
    {
        HashSet<int> targets = new HashSet<int>();
        foreach (Instruction instruction in instructions2)
        {
            OperandType opKind = instruction.OpCode.OperandType;
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

    public static ArgumentVariable[] GetMethodArguments(this MethodBase method)
    {
        var argumentList = new List<ArgumentVariable>();
        ParameterInfo[] methodParams = method.GetParameters() ?? [];
        for (int index = 0; index < methodParams.Length; index++)
        {
            ParameterInfo parameterInfo = methodParams[index];
            ArgumentVariable localVariable = new ArgumentVariable()
            {
                Index = index,
                ExpressionType = parameterInfo.ParameterType,
            };
            argumentList.Add(localVariable);
        }

        return argumentList.ToArray();
    }
}