using System.Reflection;
using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Values;
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

        foreach (var method in MethodResolver.MethodCache.Values)
        {
            if (method is CilNativeMethod cilMethod)
            {
                WriteCilMethodHeader(cilMethod);
            }
        }

        WriteMainBody(entryPoint);

        foreach (var method in MethodResolver.MethodCache.Values)
        {
            if (method is CilNativeMethod cilMethod)
            {
                WriteCilMethod(cilMethod);
                Code.AddLine();
            }
        }

        WriteStringPool();

        Code.WriteToFile();
    }

    private void WriteMainBody(string entryPoint, string args = "")
    {
        Code
            .AddLine("int main() {")
            .AddLine(entryPoint + "();")
            .AddLine("return 0;")
            .AddLine("}");
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

    private void WriteStringPool()
    {
        StringPool stringPool = StringPool.Instance;
        var coders = stringPool.Coders;
        var endPos = new List<int>();
        var joinedTexts = new List<byte>();
        var startPos = 0;
        foreach (byte[] utf8Text in stringPool.Values)
        {
            startPos += utf8Text.Length;
            joinedTexts.AddRange(utf8Text);
            endPos.Add(startPos);
        }

        Code.AddLine("namespace {")
            .AddLine(
                $"RefArr<int> _coders = std::make_shared<Arr<int>> (Arr<int>{{{string.Join(',', stringPool.Coders)}}});")
            .AddLine($"RefArr<int> _endPos = std::make_shared<Arr<int>> (Arr<int>{{{string.Join(',', endPos)}}});")
            .AddLine(
                $"RefArr<uint8_t> _joinedTexts = std::make_shared<Arr<uint8_t>> (Arr<uint8_t>{{{string.Join(',', joinedTexts)}}});")
            .AddLine("Ref<System_String> _clr_str(int index) {")
            .AddLine("    return Texts_FromIndex(index, _coders, _endPos, _joinedTexts);")
            .AddLine("}")
            .AddLine("}");
    }
}