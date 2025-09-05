using System.Text;

namespace NativeSharp.CodeGen;

public class CodeGenToFile(string FileName)
{
    private StringBuilder Text { get; } = new();

    public void WriteToFile()
    {
        File.WriteAllText(FileName, Text.ToString());
    }

    public void AddLine(string text) => Text.AppendLine(text);
}