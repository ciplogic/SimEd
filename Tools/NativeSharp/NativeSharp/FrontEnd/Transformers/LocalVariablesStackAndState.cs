using System.Reflection;
using NativeSharp.Common;
using NativeSharp.Extensions;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

class LocalVariablesStackAndState
{
    public List<IndexedVariable> LocalVariables { get; } = [];
    private readonly Stack<IValueExpression> _variableStack = new();
    public int[] _targetBranches = [];
    private int _vregIndex = 0;


    public VReg NewVirtVar(Type varType)
    {
        VReg virtVar = new VReg()
        {
            Index = _vregIndex++,
            ExpressionType = varType
        };
        LocalVariables.Add(virtVar);
        _variableStack.Push(virtVar);
        return virtVar;
    }

    public void BuildLocalVariables(MethodBase parentMethod)
    {
        _vregIndex = 0;
        _variableStack.Clear();
        LocalVariables.Clear();

        IList<LocalVariableInfo> locals = parentMethod.GetMethodBody()!.LocalVariables ?? [];
        foreach (LocalVariableInfo localVariableInfo in locals)
        {
            LocalVariable localVariable = new LocalVariable()
            {
                Index = localVariableInfo.LocalIndex,
                ExpressionType = localVariableInfo.LocalType
            };
            LocalVariables.Add(localVariable);
        }

        Instruction[] instructions2 = MethodBodyReader.GetInstructions(parentMethod);
        _targetBranches = instructions2.BuildTargetBranches();
    }

    public IValueExpression Pop() => _variableStack.Pop();
}