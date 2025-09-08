using System.Reflection;
using NativeSharp.CodeGen;
using NativeSharp.Lib;
using NativeSharp.Operations.Common;
using NativeSharp.Resolving;

namespace NativeSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        var asm = Assembly.LoadFrom("TargetApp.dll");
        var entryPoint = asm.EntryPoint!;
        
        MethodResolver.ScanAssembly(typeof(Texts).Assembly);

        MethodResolver.ResolveCilMethod(MethodResolver.Resolve(entryPoint));
        
        MethodResolver.TransformCilMethod(typeof(Texts).GetMethod("FromIndex")!);
        MethodResolver.TransformCilMethod(typeof(Texts).GetMethod("BuildSystemString")!);
        
        var codeGen = new CodeGenerator();
        codeGen.WriteMethodsAndMain(entryPoint.MangleMethodName());
        CodeGeneratorBaseTypes.GenerateNativeMappings();
    }
}