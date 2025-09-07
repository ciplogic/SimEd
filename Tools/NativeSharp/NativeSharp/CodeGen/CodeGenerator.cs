using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;
using NativeSharp.Resolving;

namespace NativeSharp.CodeGen;

public class CodeGenerator
{
    private CodeGenToFile Code { get; } = new("output.cpp");

    public void WriteMethodsAndMain()
    {
        WriteInitialCode();

        foreach (var method in MethodResolver.MethodCache.Values)
        {
            if (method is CilNativeMethod cilMethod)
            {
                WriteCilMethodHeader(cilMethod);
            }
        }

        foreach (var method in MethodResolver.MethodCache.Values)
        {
            if (method is CilNativeMethod cilMethod)
            {
                WriteCilMethod(cilMethod);
            }
        }

        Code.WriteToFile();
    }

    private void WriteInitialCode()
    {

        Code.AddLine(
            """
                 #include "native_sharp.hpp"
                 
                 namespace {
                     template <class T>
                     auto new_arr(int size) {
                         RefArr<T> result = std::make_shared<Arr<T>>();
                         result->resize(size);
                         return result;
                     }
                 }
                 
                 """);
    }

    private void WriteInstructions(BaseOp[] instructions)
    {
        foreach (BaseOp instruction in instructions)
        {
            Code.AddLine(instruction.GenCode());
        }
    }

    private void WriteCilMethodHeader(CilNativeMethod cilNativeMethod)
    {
        Code.AddLine($"{cilNativeMethod.MangledMethodHeader()};");
    }

    private void WriteCilMethod(CilNativeMethod cilNativeMethod)
    {
        var methodHeader = cilNativeMethod.MangledMethodHeader();
        Code.AddLine(methodHeader);

        Code.AddLine("{");
        WriteLocals(cilNativeMethod.Locals);
        WriteInstructions(cilNativeMethod.Instructions);
        Code.AddLine("}");
    }

    private void WriteLocals(IndexedVariable[] cilMethodLocals)
    {
        foreach (IndexedVariable localVariable in cilMethodLocals)
        {
            Code.AddLine($"{localVariable.ExpressionType.Mangle()} {localVariable.GenCodeImpl()};");
        }
    }
}