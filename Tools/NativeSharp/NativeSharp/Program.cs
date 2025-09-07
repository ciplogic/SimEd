using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Operations.Common;
using NativeSharp.Resolving;

namespace NativeSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        var asm = Assembly.LoadFrom("TargetApp.dll");
        var entryPoint = asm.EntryPoint!;

        MethodResolver.ResolveCilMethod(MethodResolver.Resolve(entryPoint));

        var codeGen = new CodeGenerator();
        codeGen.WriteMethodsAndMain();
        CodeGeneratorBaseTypes.GenerateNativeMappings();
    }
}