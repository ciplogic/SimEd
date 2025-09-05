using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.FrontEnd;
using NativeSharp.Lib;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;

namespace NativeSharp;

class MethodResolver
{
    public Dictionary<MethodBase, BaseMethod> MethodCache { get; } = [];

    static BaseMethod? ResolveSystemClrMethod(MethodInfo clrMethod)
    {
        var parmeterCount = clrMethod.GetParameters().Length;
        if (!clrMethod.IsStatic)
        {
            parmeterCount++;
        }

        var fullTargetMethodName = $"{clrMethod.DeclaringType!.FullName.Mangle()}_{clrMethod.Name}";
        var methods = typeof(ResolvedMethods)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.GetParameters().Length == parmeterCount)
            .Where(x => x.Name == fullTargetMethodName)
            .ToArray();

        if (methods.Length == 0)
        {
            return null;
        }


        return TransformCilMethod(methods[0]);
    }

    public static BaseMethod? Resolve(MethodBase clrMethod)
    {
        if (clrMethod.DeclaringType.FullName.StartsWith("System"))
        {
            var systemClrMethod = ResolveSystemClrMethod(clrMethod as MethodInfo);
            if (systemClrMethod != null)
            {
                systemClrMethod.Target = clrMethod;
            }

            return systemClrMethod;
        }

        return TransformCilMethod(clrMethod);
    }

    private static BaseMethod? TransformCilMethod(MethodBase clrMethod)
    {
        var transformer = new InstructionTransformer();
        var operations = transformer.Transform(clrMethod);
        return new CilMethod()
        {
            Locals = transformer.LocalVariablesStackAndState.LocalVariables.ToArray(),
            Args = transformer.Params.ToArray(),
            Instructions = operations,
            Target = clrMethod,
        };
    }

    public void ResolveCilMethod(BaseMethod? method)
    {
        if (method is not CilMethod cilMethod)
        {
            return;
        }

        MethodCache.TryAdd(cilMethod.Target, cilMethod);

        var callTargets = cilMethod.Instructions.OfType<CallOp>().Select(x => x.TargetMethod).ToArray();
        foreach (MethodBase target in callTargets)
        {
            var resolved = Resolve(target);
            if (resolved is not null)
            {
                MethodCache[target] = resolved!;
            }
        }
    }
}