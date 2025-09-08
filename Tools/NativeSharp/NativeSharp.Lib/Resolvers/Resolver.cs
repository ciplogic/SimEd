using System.Reflection;

namespace NativeSharp.Lib.Resolvers;

public class Resolver : IMethodResolver
{
    public CppNativeContent? Resolve(MethodBase method)
    {
        if (method.IsStatic && method.DeclaringType == typeof(Console) && method.Name == "WriteLine")
        {
            return new CppNativeContent("wsprintf(\"%s\", arg_0->Data);",
                Headers: ["stdio.h"],
                []);
        }

        return null;
    }
}