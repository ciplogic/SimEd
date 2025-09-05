using System.Reflection;

namespace NativeSharp.Operations.Common;

public class BaseMethod
{
    public MethodBase Target { get; set; } = null!;
}