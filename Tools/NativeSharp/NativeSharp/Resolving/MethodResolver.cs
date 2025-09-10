using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Common;
using NativeSharp.Extensions;
using NativeSharp.FrontEnd;
using NativeSharp.Lib;
using NativeSharp.Lib.Resolvers;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;

namespace NativeSharp.Resolving;

class MethodResolver
{
    public static Dictionary<MethodBase, BaseNativeMethod> MethodCache { get; } = [];
    public static Dictionary<MethodBase, MethodBase> RemappedMethods { get; } = [];
    public static TwoWayDictionary<Type> MappedType { get; } = new();

    public static List<IMethodResolver> AllMethodResolvers { get; } = [];

    private static BaseNativeMethod? ResolveSystemClrMethod(MethodInfo clrMethod)
    {
        if (MethodCache.TryGetValue(clrMethod, out BaseNativeMethod? method))
        {
            return method;
        }

        ParameterInfo[] parameterInfos = clrMethod.GetParameters();
        int parmeterCount = parameterInfos.Length;
        if (!clrMethod.IsStatic)
        {
            parmeterCount++;
        }

        string fullTargetMethodName = $"{clrMethod.MangleMethodName()}";
        MethodInfo? mappedMethod = typeof(ResolvedMethods)
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
        int offset = isStatic ? 0 : 1;
        for (int i = offset; i < mappedMethodInfo.Length; i++)
        {
            ParameterInfo mappedParam = mappedMethodInfo[i];
            ParameterInfo param = parameterInfos[i];
        }

        RemappedMethods[clrMethod] = mappedMethod;
        CppCodeAttribute? cppCodeAttribute = mappedMethod.GetCustomAttribute<CppCodeAttribute>();
        if (cppCodeAttribute is not null)
        {
            CppNativeMethod resolveSystemClrMethod = new(cppCodeAttribute.NativeContent)
            {
                Target = clrMethod,
                Args = clrMethod.GetMethodArguments(),
            };
            MethodCache[clrMethod] = resolveSystemClrMethod;
            return resolveSystemClrMethod;
        }

        return TransformCilMethod(clrMethod, mappedMethod);
    }

    public static BaseNativeMethod? Resolve(MethodBase clrMethod)
    {
        Type declaringType = clrMethod.DeclaringType!;
        string signature = clrMethod.MangleMethodName();
        if (signature.StartsWith("System"))
        {
            BaseNativeMethod? systemClrMethod = ResolveSystemClrMethod(clrMethod as MethodInfo);
            if (systemClrMethod != null)
            {
                systemClrMethod.Target = clrMethod;
            }

            return systemClrMethod;
        }

        return TransformCilMethod(clrMethod, clrMethod);
    }

    public static BaseNativeMethod? TransformCilMethod(MethodBase clrMethod, MethodBase? remappedClrMethod = null)
    {
        remappedClrMethod ??= clrMethod;
        InstructionTransformer transformer = new InstructionTransformer();
        CilNativeMethod transformCilMethod = new CilNativeMethod()
        {
            Target = clrMethod,
        };
        MethodCache[clrMethod] = transformCilMethod;
        BaseOp[] operations = transformer.Transform(remappedClrMethod);
        transformCilMethod.Locals = transformer.LocalVariablesStackAndState.LocalVariables.ToArray();
        transformCilMethod.Args = transformer.Params.ToArray();
        transformCilMethod.Instructions = operations;

        return transformCilMethod;
    }

    public static void ResolveMethod(MethodBase clrMethod)
    {
        if (MethodCache.ContainsKey(clrMethod))
        {
            return;
        }

        ResolveCilMethod(ResolveSystemClrMethod(clrMethod as MethodInfo));
    }

    public static void ResolveCilMethod(BaseNativeMethod? method)
    {
        if (method is not CilNativeMethod cilMethod)
        {
            return;
        }

        MethodCache.TryAdd(cilMethod.Target, cilMethod);

        MethodBase[] callTargets = cilMethod.Instructions.OfType<CallOp>().Select(x => x.TargetMethod).ToArray();
        foreach (MethodBase target in callTargets)
        {
            BaseNativeMethod? resolved = Resolve(target);
            if (resolved is not null)
            {
                MethodCache[target] = resolved!;
            }
        }
    }

    public static void ResolveAllTree(MethodBase entryPoint)
    {
        if (MethodCache.ContainsKey(entryPoint))
        {
            return;
        }
        
        var resolvedMethod = Resolve(entryPoint);
        if (resolvedMethod is not CilNativeMethod cilMethod)
        {
            return;
        }
        var methodsVoid = cilMethod.Instructions.OfType<CallOp>().Select(x=>x.TargetMethod).ToArray();
        var methodsReturn = cilMethod.Instructions.OfType<CallReturnOp>().Select(x => x.TargetMethod).ToArray();
        var joinedMethodsToResolve = methodsVoid.Concat(methodsReturn).ToArray();
        if (joinedMethodsToResolve.Length == 0)
        {
            return;
        }

        foreach (MethodBase method in joinedMethodsToResolve)
        {
            ResolveAllTree(method);
        }
        
    }
}