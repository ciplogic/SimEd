namespace SimEd.Models.Languages.Lexer;

public record struct Token(ArraySegment<char> Text, int Position, string Kind)
{
    override public string ToString()
        => $"{GetText()}: {Kind}";

    public string AText
        => GetText();

    public string GetText() => new(Text.ToArray());


    public bool IsText(ReadOnlySpan<char> text)
    {
        if (text.Length != Text.Count)
        {
            return false;
        }

        for (int index = 0; index < text.Length; index++)
        {
            char origText = Text[index];
            char ch = text[index];
            if (origText != ch)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsInTexts(string[] texts)
    {
        foreach (string text in texts)
        {
            if (IsText(text))
            {
                return true;
            }
        }

        return false;
    }
}