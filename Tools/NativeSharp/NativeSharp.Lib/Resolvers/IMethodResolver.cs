using System.Reflection;

namespace NativeSharp.Lib.Resolvers;

public interface IMethodResolver
{
    CppNativeContent? Resolve(MethodBase method);
}