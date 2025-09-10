using System.Reflection;
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
        MethodBase operand = (MethodBase)instruction.Operand;
        MethodInfo? operandAsMethodInfo = operand as MethodInfo;

        int paramCount = operandAsMethodInfo?.GetParameters().Length ?? 0;

        List<IValueExpression> args = new List<IValueExpression>();
        for (int i = 0; i < paramCount; i++)
        {
            args.Add(locals.Pop());
        }

        if (operandAsMethodInfo != null && !operandAsMethodInfo.IsStatic)
        {
            //makes sure that this pointer is also pushed for non static methods.
            args.Add(locals.Pop());
        }

        args.Reverse();

        Type returnType = operandAsMethodInfo?.ReturnType ?? typeof(void);
        VReg? returnValue = null;

        MethodResolver.ResolveMethod(operand);
        if (returnType != typeof(void))
        {
            returnValue = locals.NewVirtVar(returnType);
            return new CallReturnOp(returnValue, CallType.Static)
            {
                TargetMethod = operand,
                Args = args.ToArray()
            };
        }

        
        CallOp result = new CallOp()
        {
            CallType = CallType.Static,
            TargetMethod = operand,
            Args = args.ToArray()
        };
        return result;
    }
}