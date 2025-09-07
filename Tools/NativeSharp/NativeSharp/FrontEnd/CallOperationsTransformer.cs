using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Common;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;
using NativeSharp.Resolving;

namespace NativeSharp.FrontEnd;

static class CallOperationsTransformer
{
    public static BaseOp TransformCallOp(LocalVariablesStackAndState locals, Instruction instruction)
    {
        var opName = instruction.OpCode.Name;
        var operand = (MethodBase)instruction.Operand;
        var operandAsMethodInfo = operand as MethodInfo;
        Console.WriteLine($"Method: {operand.MangleMethodName()}");

        var paramCount = operandAsMethodInfo?.GetParameters().Length ?? 0;

        var args = new List<IValueExpression>();
        for (var i = 0; i < paramCount; i++)
        {
            args.Add(locals.Pop());
        }

        if (operandAsMethodInfo != null && !operandAsMethodInfo.IsStatic)
        {
            //makes sure that this pointer is also pushed for non static methods.
            args.Add(locals.Pop());
        }

        var returnType = operandAsMethodInfo?.ReturnType ?? typeof(void);
        VReg? returnValue = null;
        if (returnType != typeof(void))
        {
            returnValue = locals.NewVirtVar(returnType);
        }

        MethodResolver.ResolveMethod(operand);

        CallOp result = new CallOp()
        {
            CallType = CallType.Static,
            TargetMethod = operand,
            ReturnValue = returnValue,
            Args = args.ToArray()
        };
        return result;
    }
}