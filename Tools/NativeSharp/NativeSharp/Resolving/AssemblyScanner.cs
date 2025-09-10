using System.Reflection;
using NativeSharp.Lib.Resolvers;
using NativeSharp.Lib.System;

namespace NativeSharp.Resolving;

class AssemblyScanner
{
    public static void ScanAssembly(Assembly assembly)
    {
        List<IMethodResolver> resolvers = new();
        Type[] types = assembly.GetTypes()
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

        MethodResolver.AllMethodResolvers.AddRange(resolvers);
    }

    public static void DefaultMappings()
    {
        MethodResolver.MappedType[typeof(System_String)] = typeof(string);
    }
}