using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.FieldsAndIndexing;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

static class TransformNewOps
{
    public static BaseOp TransformNewDeclarations(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        string? opName = instruction.OpCode.Name;

        if (opName == "newarr")
        {
            return TransformNewArr(instruction, localVariablesStackAndState);
        }

        if (opName == "newobj")
        {
            return TransformNewObj(instruction, localVariablesStackAndState);
        }

        throw new InvalidOperationException(opName);
    }

    private static BaseOp TransformNewObj(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        ConstructorInfo constructorInfo = (ConstructorInfo)instruction.Operand;
        int argumentCount = constructorInfo.GetParameters().Length;
        List<IValueExpression> args = new List<IValueExpression>();
        for (int i = 0; i < argumentCount; i++)
        {
            args.Add(localVariablesStackAndState.Pop());
        }

        VReg result = localVariablesStackAndState.NewVirtVar(constructorInfo.DeclaringType!);
        return new NewObjOp(result, args.ToArray());
    }

    private static BaseOp TransformNewArr(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        IValueExpression popCount = localVariablesStackAndState.Pop();

        Type elementType = (Type)instruction.Operand;
        Type arrayType = elementType.MakeArrayType();
        VReg result = localVariablesStackAndState.NewVirtVar(arrayType);
        return new NewArrayOp(result, elementType, popCount);
    }
}