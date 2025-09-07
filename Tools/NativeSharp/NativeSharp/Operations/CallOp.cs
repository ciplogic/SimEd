using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;

namespace NativeSharp.Operations;

public class CallOp : BaseOp
{
    public CallType CallType { get; set; }
    public IValueExpression[] Args { get; set; } = [];
    public IRefValue? ReturnValue { get; set; } = null;

    public MethodBase TargetMethod { get; set; } = null!;

    public override string ToString() => $"call {TargetMethod.Name}";

    public override string GenCode()
    {
        string args = string.Join(", ", Args.Select(x => x.Code()));
        var result =  $"{TargetMethod.MangleMethodName()}({args});";
        if (ReturnValue is not null)
        {
            result = $"{ReturnValue.GenCode()} = {result}";
        }
        
        return result;
    }
}

public enum CallType
{
    Virtual,
    Static,
    Native,
    Dynamic,
}