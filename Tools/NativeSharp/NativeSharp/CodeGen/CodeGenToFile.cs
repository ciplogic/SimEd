using System.Text;

namespace NativeSharp.CodeGen;

public class CodeGenToFile(string fileName)
{
    public string FileName { get; } = fileName;
    private StringBuilder Text { get; } = new();

    public CodeGenToFile AddLine(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return this;
        }

        Text.AppendLine(text);
        return this;
    }

    public CodeGenToFile AddLine()
    {
        Text.AppendLine();
        return this;
    }

    public void WriteToFile()
        => File.WriteAllText(FileName, Text.ToString());
}