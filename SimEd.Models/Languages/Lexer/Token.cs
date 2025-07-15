namespace SimEd.Models.Languages.Lexer;

public record struct Token(int Kind, ArraySegment<char> Text, int Position)
{
    public override string ToString()
        => $"{GetText()}: {Kind}";

    public string AText
        => GetText();

    public string GetText() => new(Text.ToArray());


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
    private bool IsText(ReadOnlySpan<char> text)
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


}