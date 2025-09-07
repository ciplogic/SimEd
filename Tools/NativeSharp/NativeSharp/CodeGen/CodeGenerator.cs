using System.Reflection;
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
        Code.AddLine("#include \"native_sharp.hpp\"");
        WriteReferencedTypes();
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
                Code.AddLine();
            }
        }

        Code.WriteToFile();
    }

    public void WriteReferencedTypes()
    {
        var mappedTypes = MethodResolver.MappedType.Straight;
        foreach (var kv in mappedTypes)
        {
            this.Code.AddLine($"struct {kv.Value.Mangle(RefKind.Value)};");
        }

        foreach (var kv in mappedTypes)
        {
            this.Code.AddLine($"struct {kv.Value.Mangle(RefKind.Value)} {{");
            var mappedType = kv.Key;
            foreach (FieldInfo variable in mappedType.GetFields())
            {
                if (variable.IsStatic)
                {
                    continue;
                }

                Code.AddLine($"{variable.FieldType.Mangle()} {variable.Name};");
            }

            this.Code.AddLine("};");
        }
    }

    private void WriteInitialCode()
    {
        Code.AddLine(
            """
            

            namespace {

                Ref<System_String> _clr_str(int index);
                 
                template <class T>
                    RefArr<T> new_arr(int size) {
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