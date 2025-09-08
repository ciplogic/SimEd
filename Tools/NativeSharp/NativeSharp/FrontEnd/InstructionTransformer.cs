using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Extensions;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.FieldsAndIndexing;
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

        Instruction[] instructions2 = MethodBodyReader.GetInstructions(parentMethod);

        List<BaseOp> resultList = new List<BaseOp>();
        foreach (Instruction instruction in instructions2)
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
        Params.AddRange(parentMethod.GetMethodArguments());

        ReturnType = (parentMethod as MethodInfo)?.ReturnType ?? typeof(void);
    }

    private BaseOp TransformOp(Instruction instruction)
    {
        string? opName = instruction.OpCode.Name;
        object operand = instruction.Operand;
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
            return LoadOperationsTransformer.TransformLoadOp(instruction, Params, LocalVariablesStackAndState);
        }

        if (opName.StartsWith("br"))
        {
            return TransformBranchOperation(instruction, opName);
        }

        if (opName.StartsWith("conv"))
        {
            return ConvertOperationTransformer.TransformConvOperation(opName, LocalVariablesStackAndState);
        }

        if (opName.StartsWith("call"))
        {
            return CallOperationsTransformer.TransformCallOp(LocalVariablesStackAndState, instruction);
        }

        if (opName.StartsWith("stloc"))
        {
            return TransformStoreOp(instruction);
        }

        if (opName == "stfld")
        {
            return TransformStoreField(instruction, LocalVariablesStackAndState);
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

    private BaseOp TransformStoreField(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        FieldInfo fieldInfo = (FieldInfo)instruction.Operand;
        IValueExpression valueToSet = localVariablesStackAndState.Pop();
        IndexedVariable thisPtr = (IndexedVariable)localVariablesStackAndState.Pop();

        return new StoreFieldOp(thisPtr, valueToSet,  fieldInfo.Name);
    }

    private BaseOp TransformDup()
    {
        IValueExpression original = LocalVariablesStackAndState.Pop();
        VReg vreg1 = LocalVariablesStackAndState.NewVirtVar(original.ExpressionType);

        VReg vreg2 = LocalVariablesStackAndState.NewVirtVar(original.ExpressionType);
        return new DupOp(vreg1, vreg2, original);
    }

    private BaseOp TransformNewDeclarations(Instruction instruction)
    {
        string? opName = instruction.OpCode.Name;

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
        ConstructorInfo constructorInfo = (ConstructorInfo)instruction.Operand;
        int argumentCount = constructorInfo.GetParameters().Length;
        List<IValueExpression> args = new List<IValueExpression>();
        for (int i = 0; i < argumentCount; i++)
        {
            args.Add(LocalVariablesStackAndState.Pop());
        }

        VReg result = LocalVariablesStackAndState.NewVirtVar(constructorInfo.DeclaringType!);
        return new NewObjOp(result, args.ToArray());
    }

    private BaseOp TransformNewArr(Instruction instruction)
    {
        IValueExpression popCount = LocalVariablesStackAndState.Pop();

        Type elementType = (Type)instruction.Operand;
        Type arrayType = elementType.MakeArrayType();
        VReg result = LocalVariablesStackAndState.NewVirtVar(arrayType);
        return new NewArrayOp(result, elementType, popCount);
    }

    private BaseOp TransformBranchOperation(Instruction instruction, string opName)
    {
        bool isConditional = opName.StartsWith("brfalse") || opName.StartsWith("brtrue");
        Instruction targetInstruction = (Instruction)instruction.Operand;
        return new BranchOperation(targetInstruction.Offset, opName,
            isConditional ? LocalVariablesStackAndState.Pop() : null);
    }

    private BaseOp TransformBinaryOp(Instruction instruction)
    {
        IValueExpression leftOp = LocalVariablesStackAndState.Pop();
        IValueExpression rightOp = LocalVariablesStackAndState.Pop();
        return new BinaryOp()
        {
            Left = LocalVariablesStackAndState.NewVirtVar(leftOp.ExpressionType),
            LeftExpression = leftOp,
            RightExpression = rightOp,
            Operator = instruction.OpCode.Name!
        };
    }

    private BaseOp TransformLogicalBinaryOp(Instruction instruction)
    {
        IValueExpression leftOp = LocalVariablesStackAndState.Pop();
        IValueExpression rightOp = LocalVariablesStackAndState.Pop();
        return new BinaryOp()
        {
            Left = LocalVariablesStackAndState.NewVirtVar(typeof(bool)),
            LeftExpression = leftOp,
            RightExpression = rightOp,
            Operator = instruction.OpCode.Name!
        };
    }

    string[] LogicalBinaryOp = ["cgt", "ceq", "clt", "cle", "cgt.un", "clt.un", "ceq.un", "cne.un"];

    string[] BinaryOp = ["rem", "add", "sub", "mul", "div"];

    private BaseOp TransformStoreOp(Instruction instruction)
    {
        string? opName = instruction.OpCode.Name;
        string[] components = opName.Split('.');
        int index = 0;
        if (components[0] == ("stloc"))
        {
            if (!int.TryParse(components[1], out index))
            {
                if (components[1] == "s")
                {
                    LocalVariableInfo localVar = (LocalVariableInfo)instruction.Operand;
                    index = localVar.LocalIndex;
                }
                else
                {
                    index = (int)instruction.Operand;
                }
            }

            AssignOp assignOp = new AssignOp()
            {
                Left = LocalVariablesStackAndState.LocalVariables[index],
                Expression = LocalVariablesStackAndState.Pop()
            };
            return assignOp;
        }

        throw new InvalidOperationException(opName);
    }
}