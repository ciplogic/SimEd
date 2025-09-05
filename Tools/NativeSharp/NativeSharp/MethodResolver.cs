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

    private static BaseMethod? ResolveSystemClrMethod(MethodInfo clrMethod)
    {
        ParameterInfo[] parameterInfos = clrMethod.GetParameters();
        var parmeterCount = parameterInfos.Length;
        if (!clrMethod.IsStatic)
        {
            parmeterCount++;
        }

        var fullTargetMethodName = $"{clrMethod.DeclaringType!.FullName.Mangle()}_{clrMethod.Name}";
        var mappedMethod = typeof(ResolvedMethods)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.GetParameters().Length == parmeterCount)
            .Where(x => x.Name == fullTargetMethodName)
            .FirstOrDefault();

        if (mappedMethod is null)
        {
            return null;
        }
        
        ParameterInfo[] mappedMethodInfo = mappedMethod.GetParameters();
        bool isStatic = clrMethod.IsStatic;
        var offset = isStatic ? 0 : 1;
        for (int i = offset; i < mappedMethodInfo.Length; i++)
        {
            var mappedParam = mappedMethodInfo[i];
            var param = parameterInfos[i];
            if (mappedParam.ParameterType != param.ParameterType)
            {
                CppNameMangler.MappedLibToClrTypes[mappedParam.ParameterType] = param.ParameterType;
            }
        }


        return TransformCilMethod(mappedMethod);
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