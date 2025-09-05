using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Values;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

internal class InstructionTransformer
{
    public List<ArgumentVariable> Params { get; } = [];

    public readonly LocalVariablesStackAndState LocalVariablesStackAndState = new();

    private Type ReturnType { get; set; } = typeof(void);

    public BaseOp[] Transform(MethodBase parentMethod)
    {
        BuildLocalVariables(parentMethod);

        var instructions2 = MethodBodyReader.GetInstructions(parentMethod);

        var resultList = new List<BaseOp>();
        foreach (var instruction in instructions2)
        {
            if (LocalVariablesStackAndState._targetBranches.Contains(instruction.Offset))
            {
                resultList.Add(new LabelOp(instruction.Offset));
            }

            resultList.Add(TransformOp(instruction));
        }

        return resultList.ToArray();
    }

    private void BuildLocalVariables(MethodBase parentMethod)
    {
        LocalVariablesStackAndState.BuildLocalVariables(parentMethod);
        Params.Clear();

        var methodParams = parentMethod.GetParameters() ?? [];
        for (var index = 0; index < methodParams.Length; index++)
        {
            var parameterInfo = methodParams[index];
            var localVariable = new ArgumentVariable()
            {
                Index = index,
                ExpressionType = parameterInfo.ParameterType,
            };
            Params.Add(localVariable);
        }

        ReturnType = (parentMethod as MethodInfo)?.ReturnType ?? typeof(void);
    }

    private BaseOp TransformOp(Instruction instruction)
    {
        var opName = instruction.OpCode.Name;
        var operand = instruction.Operand;
        if (opName == "nop")
        {
            return new Nop();
        }

        if (opName == "ret")
        {
            if (ReturnType == typeof(void))
                return new RetOp(null);
            return new RetOp(LocalVariablesStackAndState.Pop());
        }

        if (opName.StartsWith("ld"))
        {
            return TransformLoadOp(instruction);
        }

        if (opName.StartsWith("br"))
        {
            return TransformBranchOperation(instruction, opName);
        }

        if (opName.StartsWith("call"))
        {
            return TransformCallOp(instruction);
        }

        if (opName.StartsWith("stloc"))
        {
            return TransformStoreOp(instruction);
        }

        if (opName == "stfld")
        {
            return TransformStoreField(instruction);
        }


        if (LogicalBinaryOp.Contains(opName))
        {
            return TransformLogicalBinaryOp(instruction);
        }

        if (BinaryOp.Contains(opName))
        {
            return TransformBinaryOp(instruction);
        }

        if (opName.StartsWith("new"))
        {
            return TransformNewDeclarations(instruction);
        }

        if (opName == "dup")
        {
            return TransformDup();
        }


        throw new InvalidOperationException(opName);
    }

    private BaseOp TransformStoreField(Instruction instruction)
    {
        var fieldInfo = (FieldInfo)instruction.Operand;
        var valueToSet = LocalVariablesStackAndState.Pop();
        var thisPtr = LocalVariablesStackAndState.Pop();

        return new StoreFieldOp(valueToSet, thisPtr, fieldInfo.Name);
    }

    private BaseOp TransformDup()
    {
        var original = LocalVariablesStackAndState.Pop();
        var vreg1 = NewVirtVar(original.ExpressionType);

        var vreg2 = NewVirtVar(original.ExpressionType);
        return new DupOp(vreg1, vreg2, original);
    }

    private BaseOp TransformNewDeclarations(Instruction instruction)
    {
        var opName = instruction.OpCode.Name;

        if (opName == "newarr")
        {
            return TransformNewArr(instruction);
        }

        if (opName == "newobj")
        {
            return TransformNewObj(instruction);
        }

        throw new InvalidOperationException(opName);
    }

    private BaseOp TransformNewObj(Instruction instruction)
    {
        var constructorInfo = (ConstructorInfo)instruction.Operand;
        var argumentCount = constructorInfo.GetParameters().Length;
        var args = new List<IValueExpression>();
        for (var i = 0; i < argumentCount; i++)
        {
            args.Add(LocalVariablesStackAndState.Pop());
        }

        var result = NewVirtVar(constructorInfo.DeclaringType!);
        return new NewObjOp(result, args.ToArray());
    }

    private BaseOp TransformNewArr(Instruction instruction)
    {
        var popCount = LocalVariablesStackAndState.Pop();

        var elementType = (Type)instruction.Operand;
        var result = NewVirtVar(elementType);
        return new NewArrayOp(result, elementType, popCount);
    }

    private BaseOp TransformBranchOperation(Instruction instruction, string opName)
    {
        var isConditional = opName.StartsWith("brfalse") || opName.StartsWith("brtrue");
        var targetInstruction = (Instruction)instruction.Operand;
        return new BranchOperation(targetInstruction.Offset, opName,
            isConditional ? LocalVariablesStackAndState.Pop() : null);
    }

    private BaseOp TransformBinaryOp(Instruction instruction)
    {
        var leftOp = LocalVariablesStackAndState.Pop();
        var rightOp = LocalVariablesStackAndState.Pop();
        return new BinaryOp()
        {
            Left = NewVirtVar(leftOp.ExpressionType),
            LeftExpression = leftOp,
            RightExpression = rightOp,
            Operator = instruction.OpCode.Name!
        };
    }

    private BaseOp TransformLogicalBinaryOp(Instruction instruction)
    {
        var leftOp = LocalVariablesStackAndState.Pop();
        var rightOp = LocalVariablesStackAndState.Pop();
        return new BinaryOp()
        {
            Left = NewVirtVar(typeof(bool)),
            LeftExpression = leftOp,
            RightExpression = rightOp,
            Operator = instruction.OpCode.Name!
        };
    }

    string[] LogicalBinaryOp = ["cgt", "ceq", "clt", "cle", "cgt.un", "clt.un", "ceq.un", "cne.un"];

    string[] BinaryOp = ["rem", "add", "sub", "mul", "div"];

    private BaseOp TransformStoreOp(Instruction instruction)
    {
        var opName = instruction.OpCode.Name;
        var components = opName.Split('.');
        int index = 0;
        if (components[0] == ("stloc"))
        {
            if (!int.TryParse(components[1], out index))
            {
                if (components[1] == "s")
                {
                    var localVar = (LocalVariableInfo)instruction.Operand;
                    index = localVar.LocalIndex;
                }
                else
                {
                    index = (int)instruction.Operand;
                }
            }

            var assignOp = new AssignOp()
            {
                Left = LocalVariablesStackAndState.LocalVariables[index],
                Expression = LocalVariablesStackAndState.Pop()
            };
            return assignOp;
        }

        throw new InvalidOperationException(opName);
    }

    private BaseOp TransformCallOp(Instruction instruction)
    {
        var opName = instruction.OpCode.Name;
        var operand = (MethodBase)instruction.Operand;
        var operandAsMethodInfo = operand as MethodInfo;

        var paramCount = operandAsMethodInfo?.GetParameters().Length ?? 0;

        var args = new List<IValueExpression>();
        for (var i = 0; i < paramCount; i++)
        {
            args.Add(LocalVariablesStackAndState.Pop());
        }

        if (operandAsMethodInfo != null && !operandAsMethodInfo.IsStatic)
        {
            //makes sure that this pointer is also pushed for non static methods.
            args.Add(LocalVariablesStackAndState.Pop());
        }

        var returnType = operandAsMethodInfo?.ReturnType ?? typeof(void);
        VReg? returnValue = null;
        if (returnType != typeof(void))
        {
            returnValue = NewVirtVar(returnType);
        }

        CallOp result = new CallOp()
        {
            CallType = CallType.Static,
            TargetMethod = operand,
            ReturnValue = returnValue,
            Args = args.ToArray()
        };
        return result;
    }

    private BaseOp TransformLoadOp(Instruction instruction)
    {
        var opName = instruction.OpCode.Name;
        var operand = instruction.Operand;

        if (opName.StartsWith("ldarg"))
        {
            return ParseLoadArgument(instruction, opName);
        }

        if (opName.StartsWith("ldc"))
        {
            return ExtractLoadConstant(instruction, opName);
        }

        if ((opName.StartsWith("ldloc")))
        {
            return ExtractLoadLocalVariable(instruction);
        }

        if (opName == "ldfld")
        {
            return ExtractField(instruction);
        }

        switch (opName)
        {
            case "ldstr":
            {
                var constValue = ConstantValueExpression.Create((string)instruction.Operand);
                return ExtractAssignFromConstant(constValue);
            }
            case "ldloca":
            case "ldloca.s":
            {
                var operandAsInt = OperandAsInt(operand);
                var localVar = LocalVariablesStackAndState.LocalVariables[operandAsInt];
                return new AssignOp()
                {
                    Left = NewVirtVar(localVar.ExpressionType),
                    Expression = localVar
                };
            }
            default:
                throw new InvalidOperationException(opName);
        }
    }

    private BaseOp ExtractField(Instruction instruction)
    {
        var thisPtr = LocalVariablesStackAndState.Pop();
        FieldInfo fieldInfo = (FieldInfo)instruction.Operand;
        var resultVar = NewVirtVar(fieldInfo.FieldType);
        return new LoadFieldOp(thisPtr, fieldInfo.Name, resultVar);
    }

    private BaseOp ExtractLoadLocalVariable(Instruction instruction)
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
            Left = NewVirtVar(LocalVariablesStackAndState.LocalVariables[index].ExpressionType),
            Expression = LocalVariablesStackAndState.LocalVariables[index]
        };
    }

    private BaseOp ParseLoadArgument(Instruction instruction, string opName)
    {
        int index = 0;
        if (!int.TryParse(opName.Substring(6), out index))
        {
            index = (int)instruction.Operand;
        }

        return new AssignOp()
        {
            Left = NewVirtVar(Params[index].ExpressionType),
            Expression = Params[index]
        };
    }

    private BaseOp ExtractLoadConstant(Instruction instruction, string opName)
    {
        int index = 0;
        if (opName.Length < 7 || !int.TryParse(opName.Substring(7), out index))
        {
            index = (int)instruction.Operand;
        }

        var constValue = ConstantValueExpression.Create(index);
        return ExtractAssignFromConstant(constValue);
    }

    int OperandAsInt(object operand)
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

    private BaseOp ExtractAssignFromConstant(ConstantValueExpression constValueExpression)
    {
        var virtVar = NewVirtVar(constValueExpression.ExpressionType);
        var assignOp = new AssignOp()
        {
            Left = virtVar,
            Expression = constValueExpression
        };
        return assignOp;
    }

    private VReg NewVirtVar(Type varType)
        => LocalVariablesStackAndState.NewVirtVar(varType);
}
