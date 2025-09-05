using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;

namespace NativeSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        var asm = Assembly.LoadFrom("TargetApp.dll");
        var entryPoint = asm.EntryPoint!;

        var methodResolver = new MethodResolver();
        methodResolver.ResolveCilMethod(MethodResolver.Resolve(entryPoint));

        var codeGen = new CodeGenerator();
        codeGen.WriteInitialCode();

        foreach (var method in methodResolver.MethodCache.Values)
        {
            if (method is CilMethod cilMethod)
            {
                codeGen.WriteCilMethodHeader(cilMethod);
            }
        }

        foreach (var method in methodResolver.MethodCache.Values)
        {
            if (method is CilMethod cilMethod)
            {
                codeGen.WriteCilMethod(cilMethod);
            }
        }

        codeGen.WriteCode();

        CodeGeneratorBaseTypes.GenerateNativeMappings();
    }
}