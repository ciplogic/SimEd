using System.Reflection;
using NativeSharp.FrontEnd;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;
using NativeSharp.Resolving;

namespace NativeSharp.CodeGen;

public class CodeGenerator
{
    private CodeGenToFile Code { get; } = new("output.cpp");

    public void WriteMethodsAndMain(string entryPoint)
    {
        Code.AddLine("#include \"native_sharp.hpp\"");
        WriteReferencedTypes();
        WriteInitialCode();

        foreach (BaseNativeMethod method in MethodResolver.MethodCache.Values)
        {
            WriteCilMethodHeader(method);
        }

        WriteMainBody(entryPoint);

        foreach (BaseNativeMethod method in MethodResolver.MethodCache.Values)
        {
            if (method is CilNativeMethod cilMethod)
            {
                WriteCilMethod(cilMethod);
            }
            else if (method is CppNativeMethod cppMethod)
            {
                WriteCppMethod(cppMethod);
            }

            Code.AddLine();
        }

        WriteStringPool();

        Code.WriteToFile();
    }

    private void WriteCppMethod(CppNativeMethod cppMethod)
    {
        var code = cppMethod.Content;
        Code.AddLine($"{cppMethod.MangledMethodHeader()} {{");
        Code.AddLine(code.MethodBody);
        Code.AddLine("}");
    }

    private void WriteMainBody(string entryPoint, string args = "") =>
        Code
            .AddLine("int main() {")
            .AddLine(entryPoint + "();")
            .AddLine("return 0;")
            .AddLine("}");

    public void WriteReferencedTypes()
    {
        Dictionary<Type, Type> mappedTypes = MethodResolver.MappedType.Straight;
        foreach (KeyValuePair<Type, Type> kv in mappedTypes)
        {
            Code.AddLine($"struct {kv.Value.Mangle(RefKind.Value)};");
        }

        foreach (KeyValuePair<Type, Type> kv in mappedTypes)
        {
            Code.AddLine($"struct {kv.Value.Mangle(RefKind.Value)} {{");
            Type mappedType = kv.Key;
            foreach (FieldInfo variable in mappedType.GetFields())
            {
                if (variable.IsStatic)
                {
                    continue;
                }

                Code.AddLine($"{variable.FieldType.Mangle()} {variable.Name};");
            }

            Code.AddLine("};");
        }
    }

    private void WriteInitialCode()
    {
        Code.AddLine(
            """
            namespace {
                Ref<System_String> _clr_str(int index);
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

    private void WriteCilMethodHeader(BaseNativeMethod cilNativeMethod)
    {
        Code.AddLine($"{cilNativeMethod.MangledMethodHeader()};");
    }

    private void WriteCilMethod(CilNativeMethod cilNativeMethod)
    {
        string methodHeader = cilNativeMethod.MangledMethodHeader();
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

    private void WriteStringPool()
    {
        StringPool stringPool = StringPool.Instance;
        List<int> startPositions = [];
        List<int> lenPos = [];
        List<byte> joinedTexts = [];
        int startPos = 0;
        foreach (byte[] utf8Text in stringPool.Values)
        {
            
            startPositions.Add(startPos);
            lenPos.Add(utf8Text.Length);
            startPos += utf8Text.Length;
            joinedTexts.AddRange(utf8Text);
        }

        Code
            .AddLine("namespace {")
            .AddLine($"    RefArr<int> _coders = new_ref_data<Arr<int>> ({{{string.Join(',', stringPool.Coders)}}});")
            .AddLine($"    RefArr<int> _startPos = new_ref_data<Arr<int>> ({{{string.Join(',', startPositions)}}});")
            .AddLine($"    RefArr<int> _lengths = new_ref_data<Arr<int>> ({{{string.Join(',', lenPos)}}});")
            .AddLine($"    RefArr<uint8_t> _joinedTexts = new_ref_data<Arr<uint8_t>> ({{{string.Join(',', joinedTexts)}}});")
            .AddLine("""
                         Ref<System_String> _clr_str(int index) {
                            return Texts_FromIndex(index, _coders, _startPos, _lengths, _joinedTexts);
                         }
                     }
                     """);
    }
}