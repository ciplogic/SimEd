using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Operations;
using NativeSharp.Operations.Values;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

static class LoadOperationsTransformer
{
    public static BaseOp TransformLoadOp(Instruction instruction, List<ArgumentVariable> argumentVariables,
        LocalVariablesStackAndState variablesStackAndState)
    {
        var opName = instruction.OpCode.Name;
        var operand = instruction.Operand;

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

        switch (opName)
        {
            case "ldstr":
            {
                var constValue = ConstantValueExpression.Create((string)instruction.Operand);
                return ExtractAssignFromConstant(constValue, variablesStackAndState);
            }
            case "ldloca":
            case "ldloca.s":
            {
                var operandAsInt = OperandAsInt(operand);
                var localVar = variablesStackAndState.LocalVariables[operandAsInt];
                return new AssignOp()
                {
                    Left = variablesStackAndState.NewVirtVar(localVar.ExpressionType),
                    Expression = localVar
                };
            }
            default:
                throw new InvalidOperationException(opName);
        }
    }

    private static BaseOp ParseLoadLen(LocalVariablesStackAndState variablesStackAndState)
    {
        var arrVar = (IndexedVariable)variablesStackAndState.Pop();
        var resultVar = variablesStackAndState.NewVirtVar(typeof(uint));
        return new LdLenOp(resultVar, arrVar);
    }

    private static BaseOp ExtractField(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        var thisPtr = localVariablesStackAndState.Pop();
        FieldInfo fieldInfo = (FieldInfo)instruction.Operand;
        var resultVar = localVariablesStackAndState.NewVirtVar(fieldInfo.FieldType);
        return new LoadFieldOp(thisPtr, fieldInfo.Name, resultVar);
    }


    public static BaseOp ParseLoadArgument(Instruction instruction, string opName,
        LocalVariablesStackAndState localVariablesStackAndState, List<ArgumentVariable> argumentVariables)
    {
        int index = 0;
        if (!int.TryParse(opName.Substring(6), out index))
        {
            index = (int)instruction.Operand;
        }

        return new AssignOp()
        {
            Left = localVariablesStackAndState.NewVirtVar(argumentVariables[index].ExpressionType),
            Expression = argumentVariables[index]
        };
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
        var opName = instruction.OpCode.Name;
        var split = opName.Split('.');
        var index = -1;
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

        return new AssignOp()
        {
            Left = localVariablesStackAndState.NewVirtVar(localVariablesStackAndState.LocalVariables[index]
                .ExpressionType),
            Expression = localVariablesStackAndState.LocalVariables[index]
        };
    }

    public static BaseOp ExtractAssignFromConstant(ConstantValueExpression constValueExpression,
        LocalVariablesStackAndState localVariablesStackAndState)
    {
        var virtVar = localVariablesStackAndState.NewVirtVar(constValueExpression.ExpressionType);
        var assignOp = new AssignOp()
        {
            Left = virtVar,
            Expression = constValueExpression
        };
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

        var constValue = ConstantValueExpression.Create(index);
        return ExtractAssignFromConstant(constValue,
            localVariablesStackAndState);
    }
}