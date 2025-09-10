using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.FieldsAndIndexing;
using NativeSharp.Operations.Values;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

static class LoadOperationsTransformer
{
    public static BaseOp TransformLoadOp(Instruction instruction, List<ArgumentVariable> argumentVariables,
        LocalVariablesStackAndState variablesStackAndState)
    {
        string? opName = instruction.OpCode.Name;
        object operand = instruction.Operand;

        if (opName.StartsWith("ldarg"))
        {
            return ParseLoadArgument(instruction, opName, variablesStackAndState,
                argumentVariables);
        }

        if (opName == "ldlen")
            return ParseLoadLen(variablesStackAndState);

        if (opName.StartsWith("ldc"))
        {
            return ExtractLoadConstant(instruction, opName, variablesStackAndState);
        }

        if ((opName.StartsWith("ldloc")))
        {
            return ExtractLoadLocalVariable(instruction, variablesStackAndState);
        }

        if (opName == "ldfld")
        {
            return ExtractField(instruction, variablesStackAndState);
        }


        if (opName.StartsWith("ldelem"))
        {
            return ExtractLoadElement(instruction, variablesStackAndState);
        }

        switch (opName)
        {
            case "ldstr":
            {
                ConstantValueExpression constValue = ConstantValueExpression.Create((string)instruction.Operand);
                return ExtractAssignFromConstant(constValue, variablesStackAndState);
            }
            case "ldloca":
            case "ldloca.s":
            {
                int operandAsInt = OperandAsInt(operand);
                IndexedVariable localVar = variablesStackAndState.LocalVariables[operandAsInt];
                var variableToAssign = variablesStackAndState.NewVirtVar(localVar.ExpressionType);
                return new AssignOp(variableToAssign, localVar);
            }
            default:
                throw new InvalidOperationException(opName);
        }
    }

    private static BaseOp ExtractLoadElement(Instruction instruction,
        LocalVariablesStackAndState variablesStackAndState)
    {
        IValueExpression index = variablesStackAndState.Pop();
        IndexedVariable array = (IndexedVariable)variablesStackAndState.Pop();

        VReg resultElement = variablesStackAndState.NewVirtVar(array.ExpressionType.GetElementType());
        LoadElementOp op = new LoadElementOp(resultElement, array, index);
        return op;
    }

    private static BaseOp ParseLoadLen(LocalVariablesStackAndState variablesStackAndState)
    {
        IndexedVariable arrVar = (IndexedVariable)variablesStackAndState.Pop();
        VReg resultVar = variablesStackAndState.NewVirtVar(typeof(uint));
        return new LdLenOp(resultVar, arrVar);
    }

    private static BaseOp ExtractField(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        IndexedVariable thisPtr = (IndexedVariable)localVariablesStackAndState.Pop();
        FieldInfo fieldInfo = (FieldInfo)instruction.Operand;
        VReg resultVar = localVariablesStackAndState.NewVirtVar(fieldInfo.FieldType);
        return new LoadFieldOp(thisPtr, fieldInfo.Name, resultVar);
    }


    public static BaseOp ParseLoadArgument(Instruction instruction, string opName,
        LocalVariablesStackAndState localVariablesStackAndState, List<ArgumentVariable> argumentVariables)
    {
        string[] components = opName.Split('.');
        int index = 0;
        if (!int.TryParse(components[1], out index))
        {
            ParameterInfo? paramInfo = instruction.Operand as ParameterInfo;
            index = paramInfo?.Position ?? (int)instruction.Operand;
        }

        VReg left = localVariablesStackAndState.NewVirtVar(argumentVariables[index].ExpressionType);

        return new AssignOp(left, argumentVariables[index]);
    }


    public static int OperandAsInt(object operand)
    {
        if (operand is int)
        {
            return (int)operand;
        }

        if (operand is LocalVariableInfo localVar)
        {
            return localVar.LocalIndex;
        }

        throw new InvalidOperationException();
    }


    public static BaseOp ExtractLoadLocalVariable(Instruction instruction,
        LocalVariablesStackAndState localVariablesStackAndState)
    {
        string? opName = instruction.OpCode.Name;
        string[] split = opName.Split('.');
        int index = -1;
        if (split.Length == 2)
        {
            if (!int.TryParse(split[1], out index))
            {
                index = OperandAsInt(instruction.Operand);
            }
        }
        else
        {
            index = (int)instruction.Operand;
        }

        var expressionType = localVariablesStackAndState.LocalVariables[index].ExpressionType;
        VReg left = localVariablesStackAndState.NewVirtVar(expressionType);

        var expression = localVariablesStackAndState.LocalVariables[index];
        return new AssignOp(left, expression);
    }

    public static BaseOp ExtractAssignFromConstant(ConstantValueExpression constValueExpression,
        LocalVariablesStackAndState localVariablesStackAndState)
    {
        VReg virtVar = localVariablesStackAndState.NewVirtVar(constValueExpression.ExpressionType);
        AssignOp assignOp = new AssignOp(virtVar, constValueExpression);
        return assignOp;
    }

    public static BaseOp ExtractLoadConstant(Instruction instruction, string opName,
        LocalVariablesStackAndState localVariablesStackAndState)
    {
        int index = 0;
        if (opName.Length < 7 || !int.TryParse(opName.Substring(7), out index))
        {
            index = (int)instruction.Operand;
        }

        ConstantValueExpression constValue = ConstantValueExpression.Create(index);
        return ExtractAssignFromConstant(constValue,
            localVariablesStackAndState);
    }
}