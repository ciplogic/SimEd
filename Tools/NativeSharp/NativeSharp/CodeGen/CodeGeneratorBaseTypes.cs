namespace NativeSharp.CodeGen;

internal static class CodeGeneratorBaseTypes
{
    public static void GenerateNativeMappings()
    {
        CodeGenToFile nativeSharpPrimitives = new CodeGenToFile("native_sharp_primitives.hpp");
        nativeSharpPrimitives.AddLine("#pragma once");
        nativeSharpPrimitives.AddLine("");
        nativeSharpPrimitives.AddLine("#include <cstdint>");
        nativeSharpPrimitives.AddLine("");
        AddMappedType(typeof(int), "int", nativeSharpPrimitives);
        AddMappedType(typeof(uint), "uint32_t", nativeSharpPrimitives);
        AddMappedType(typeof(byte), "uint8_t", nativeSharpPrimitives);
        AddMappedType(typeof(bool), "bool", nativeSharpPrimitives);
        AddMappedType(typeof(float), "float", nativeSharpPrimitives);
        AddMappedType(typeof(double), "double", nativeSharpPrimitives);
        AddMappedType(typeof(void), "void", nativeSharpPrimitives);
        nativeSharpPrimitives.WriteToFile();
    }

    static void AddMappedType(Type clrType, string mappedNativeType, CodeGenToFile codeGenToFile)
        => codeGenToFile.AddLine($"using {clrType.Mangle()} = {mappedNativeType};");
}