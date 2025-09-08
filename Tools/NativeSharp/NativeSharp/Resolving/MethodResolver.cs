using System.Reflection;
using NativeSharp.CodeGen;
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

    static List<IMethodResolver> AllMethodResolvers { get; } = [];

    private static BaseNativeMethod? ResolveSystemClrMethod(MethodInfo clrMethod)
    {
        if (MethodCache.TryGetValue(clrMethod, out var method))
        {
            return method;
        }

        ParameterInfo[] parameterInfos = clrMethod.GetParameters();
        var parmeterCount = parameterInfos.Length;
        if (!clrMethod.IsStatic)
        {
            parmeterCount++;
        }

        var fullTargetMethodName = $"{clrMethod.MangleMethodName()}";
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
                MappedType[mappedParam.ParameterType] = param.ParameterType;
            }
        }

        RemappedMethods[clrMethod] = mappedMethod;
        var cppCodeAttribute = mappedMethod.GetCustomAttribute<CppCodeAttribute>();
        if (cppCodeAttribute is not null)
        {
            return new CppNativeMethod(cppCodeAttribute.NativeContent)
            {
                Target = clrMethod,
                Args = [],
            };
        }

        return TransformCilMethod(clrMethod, mappedMethod);
    }

    public static BaseNativeMethod? Resolve(MethodBase clrMethod)
    {
        Type declaringType = clrMethod.DeclaringType!;
        var signature = clrMethod.MangleMethodName();
        if (signature.StartsWith("System"))
        {
            var systemClrMethod = ResolveSystemClrMethod(clrMethod as MethodInfo);
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
        var transformer = new InstructionTransformer();
        var transformCilMethod = new CilNativeMethod()
        {
            Target = clrMethod,
        };
        MethodCache[clrMethod] = transformCilMethod;
        var operations = transformer.Transform(remappedClrMethod);
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

    public static void ScanAssembly(Assembly assembly)
    {
        List<IMethodResolver> resolvers = new();
        var types = assembly.GetTypes()
            .Where(it => it is { IsAbstract: false, IsInterface: false })
            .ToArray();
        foreach (Type type in types)
        {
            if (type.IsAssignableTo(typeof(IMethodResolver)))
            {
                IMethodResolver resolver = (IMethodResolver)Activator.CreateInstance(type)!;
                resolvers.Add(resolver);
            }
        }

        AllMethodResolvers.AddRange(resolvers);
    }
}