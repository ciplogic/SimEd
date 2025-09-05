using System.Reflection;
using NativeSharp.Operations.Common;

namespace NativeSharp.CodeGen;

internal static class CppNameMangler
{
    public static Dictionary<Type, Type> MappedLibToClrTypes { get; } = new();

    public static string Mangle(this Type clrType, RefKind refKind = RefKind.Default)
    {
        if (MappedLibToClrTypes.TryGetValue(clrType, out var mappedClrType))
        {
            clrType = mappedClrType;
        }

        var fullName = clrType.FullName!;
        refKind = refKind == RefKind.Default ? clrType.IsValueType ? RefKind.Value : RefKind.Ref : refKind;
        var resultMangle = Mangle(fullName);
        return refKind switch
        {
            RefKind.Ref => $"Ref<{resultMangle}>",
            RefKind.Ptr => $"{resultMangle}*",
            _ => resultMangle
        };
    }

    public static string Mangle(this string fullName) => fullName.Replace('.', '_');

    public static string MangleMethodName(this MethodBase method)
    {
        var declaringType = method.DeclaringType.Mangle(RefKind.Value);
        var methodName = "ctor";
        if (method is MethodInfo methodInfo)
        {
            methodName = methodInfo.Name;
        }

        return $"{declaringType}_{methodName}";
    }


    public static string MangledMethodHeader(this CilMethod cilMethod)
    {
        var args = string.Join(", ", cilMethod.Args.Select(x => $"{x.ExpressionType.Mangle()} {x.GenCodeImpl()}"));
        string methodHeader =
            $"{cilMethod.Target.MangleMethodReturnType()} {cilMethod.Target.MangleMethodName()}({args})";
        return methodHeader;
    }

    public static string MangleMethodReturnType(this MethodBase method)
    {
        if (method is MethodInfo methodInfo)
        {
            return methodInfo.ReturnType.Mangle();
        }

        return typeof(void).Mangle();
    }
}

internal enum RefKind
{
    Default,
    Ref,
    Value,
    Ptr
}