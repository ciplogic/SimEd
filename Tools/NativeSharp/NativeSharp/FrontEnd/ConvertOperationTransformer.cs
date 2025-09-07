using NativeSharp.Operations;

namespace NativeSharp.FrontEnd;

static class ConvertOperationTransformer
{
    

    public static BaseOp TransformConvOperation(string opName, LocalVariablesStackAndState localVariablesStackAndState)
    {
        var localVar = localVariablesStackAndState.Pop();
        var mappedSuffix = opName.Split('.')[1];
        Type targetType = mappedSuffix switch
        {
            "i4" => typeof(int),
            "i8" => typeof(long),
            _ => throw new InvalidOperationException($"Cannot cast to: {mappedSuffix}")
        };

        var resultVar = localVariablesStackAndState.NewVirtVar(targetType);
        return new ConvOp(opName, resultVar, localVar);
    }
}