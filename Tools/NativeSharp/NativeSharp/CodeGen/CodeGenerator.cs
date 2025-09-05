using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.CodeGen;

public class CodeGenerator
{
    private CodeGenToFile Code { get; } = new("output.cpp");

    public void WriteInitialCode()
    {
        Code.AddLine("#include \"native_sharp.hpp\"");
    }

    private void WriteInstructions(BaseOp[] instructions)
    {
        foreach (BaseOp instruction in instructions)
        {
            Code.AddLine(instruction.GenCode());
        }
    }

    public void WriteCilMethodHeader(CilMethod cilMethod)
    {
        Code.AddLine($"{cilMethod.MangledMethodHeader()};");
    }

    public void WriteCilMethod(CilMethod cilMethod)
    {
        var methodHeader = cilMethod.MangledMethodHeader();
        Code.AddLine(methodHeader);

        Code.AddLine("{");
        WriteLocals(cilMethod.Locals);
        WriteInstructions(cilMethod.Instructions);
        Code.AddLine("}");
    }

    private void WriteLocals(IndexedVariable[] cilMethodLocals)
    {
        foreach (IndexedVariable localVariable in cilMethodLocals)
        {
            Code.AddLine($"{localVariable.ExpressionType.Mangle()} {localVariable.GenCodeImpl()};");
        }
    }

    public void WriteCode() => Code.WriteToFile();
}