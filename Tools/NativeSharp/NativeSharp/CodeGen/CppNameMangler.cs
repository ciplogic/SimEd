using System.Reflection;
using NativeSharp.Operations.Common;
using NativeSharp.Resolving;

namespace NativeSharp.CodeGen;

internal static class CppNameMangler
{
    public static string Mangle(this Type clrType, RefKind refKind = RefKind.Default)
    {
        if (MethodResolver.MappedType.TryGetValue(clrType, out Type mappedClrType))
        {
            clrType = mappedClrType;
        }

        if (clrType.IsArray)
        {
            return $"RefArr<{clrType.GetElementType()!.Mangle()}>";
        }
        string fullName = clrType.FullName!;
        refKind = refKind == RefKind.Default ? clrType.IsValueType ? RefKind.Value : RefKind.Ref : refKind;
        string resultMangle = Mangle(fullName);
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
        string declaringType = method.DeclaringType.Mangle(RefKind.Value);
        string methodName = "ctor";
        if (method is MethodInfo methodInfo)
        {
            methodName = methodInfo.Name;
        }

        return $"{declaringType}_{methodName}";
    }

    public static string MangledMethodHeader(this BaseNativeMethod cilNativeMethod)
    {
        string args = string.Join(", ",
            cilNativeMethod.Args.Select(x => $"{x.ExpressionType.Mangle()} {x.GenCodeImpl()}"));
        string methodHeader =
            $"{cilNativeMethod.Target.MangleMethodReturnType()} {cilNativeMethod.Target.MangleMethodName()}({args})";
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