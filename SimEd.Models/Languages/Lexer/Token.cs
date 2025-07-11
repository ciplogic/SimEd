namespace SimEd.Models.Languages.Lexer;

public record struct Token(ArraySegment<char> Text, int Position, string Kind)
{
    public override string ToString()
        => $"{GetText()}: {Kind}";

    public string AText
        => GetText();

    public string GetText() => new(Text.ToArray());


    private bool IsText(ReadOnlySpan<char> text)
    {
        if (text.Length != Text.Count)
        {
            return false;
        }

        for (var index = 0; index < text.Length; index++)
        {
            var origText = Text[index];
            var ch = text[index];
            if (origText != ch)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsInTexts(string[] texts)
    {
        foreach (var text in texts)
        {
            if (IsText(text))
            {
                return true;
            }
        }

        return false;
    }
}