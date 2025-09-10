using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Extensions;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.FieldsAndIndexing;
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

            var transformedOp = TransformOp(instruction);
            if (transformedOp is CompositeOp compositeOp)
            {
                foreach (BaseOp baseOp in compositeOp.Ops)
                {
                    resultList.Add(baseOp);
                }
            }
            else
            {
                resultList.Add(transformedOp);
            }
        }

        BaseOp[] ops = resultList.ToArray();
        PhiFixup.FixupMerges(ops);

        return ops;
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
        if (opName == "nop")
        {
            return new CompositeOp([]);
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

        if (opName.StartsWith("stelem"))
        {
            return TransformStoreElement(instruction, LocalVariablesStackAndState);
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
            return TransformNewOps.TransformNewDeclarations(instruction, LocalVariablesStackAndState);
        }

        if (opName == "dup")
        {
            return TransformDup();
        }


        throw new InvalidOperationException(opName);
    }

    private BaseOp TransformStoreElement(Instruction instruction,
        LocalVariablesStackAndState localVariablesStackAndState)
    {
        var valueToSet = localVariablesStackAndState.Pop();
        var index = localVariablesStackAndState.Pop();
        var arrPtr = (IndexedVariable)localVariablesStackAndState.Pop();

        return new StoreElementOp(arrPtr, index, valueToSet);
    }

    private BaseOp TransformStoreField(Instruction instruction, LocalVariablesStackAndState localVariablesStackAndState)
    {
        FieldInfo fieldInfo = (FieldInfo)instruction.Operand;
        IValueExpression valueToSet = localVariablesStackAndState.Pop();
        IndexedVariable thisPtr = (IndexedVariable)localVariablesStackAndState.Pop();

        return new StoreFieldOp(thisPtr, valueToSet, fieldInfo.Name);
    }

    private BaseOp TransformDup()
    {
        IValueExpression original = LocalVariablesStackAndState.Pop();
        VReg vreg1 = LocalVariablesStackAndState.NewVirtVar(original.ExpressionType);
        VReg vreg2 = LocalVariablesStackAndState.NewVirtVar(original.ExpressionType);
        AssignOp assignOp1 = new(vreg1, original);
        AssignOp assignOp2 = new(vreg2, original);

        return new CompositeOp([assignOp1, assignOp2]);
    }

    private BaseOp TransformBranchOperation(Instruction instruction, string opName)
    {
        bool isConditional = opName.StartsWith("brfalse") || opName.StartsWith("brtrue");
        Instruction targetInstruction = (Instruction)instruction.Operand;
        int targetInstructionOffset = targetInstruction.Offset;
        if (!isConditional)
        {
            return new GotoOp(targetInstructionOffset);
        }

        return new BranchOp(targetInstructionOffset, opName,
            isConditional ? LocalVariablesStackAndState.Pop() : null);
    }

    private BaseOp TransformBinaryOp(Instruction instruction)
    {
        IValueExpression rightOp = LocalVariablesStackAndState.Pop();
        IValueExpression leftOp = LocalVariablesStackAndState.Pop();
        var left = LocalVariablesStackAndState.NewVirtVar(leftOp.ExpressionType);
        return new BinaryOp(left)
        {
            LeftExpression = leftOp,
            RightExpression = rightOp,
            Operator = instruction.OpCode.Name!
        };
    }

    private BaseOp TransformLogicalBinaryOp(Instruction instruction)
    {
        IValueExpression rightOp = LocalVariablesStackAndState.Pop();
        IValueExpression leftOp = LocalVariablesStackAndState.Pop();
        var left = LocalVariablesStackAndState.NewVirtVar(leftOp.ExpressionType);
        return new BinaryOp(left)
        {
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

            var leftVar = LocalVariablesStackAndState.LocalVariables[index];

            var expression = LocalVariablesStackAndState.Pop();
            AssignOp assignOp = new(leftVar, expression);
            return assignOp;
        }

        throw new InvalidOperationException(opName);
    }
}