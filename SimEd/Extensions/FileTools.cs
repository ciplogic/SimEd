using System.Text;

namespace SimEd.Extensions;

public static class FileTools
{
    public static Encoding GetEncoding(string path)
    {
        using StreamReader reader = new (path, Encoding.Default, true);
        if (reader.Peek() >= 0)
        {
            reader.Read();
        }

        return reader.CurrentEncoding;
    }

}