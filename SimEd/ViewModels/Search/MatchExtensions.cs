namespace SimEd.ViewModels.Search;

static class MatchExtensions
{
    public static bool IsSmartMatch(this string token, string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            return true;
        }

        var wordsSplit = SplitIntoTokens(token);
        var currentWordIndex = 0;
        var indexInWord = 0;
        var i = 0;
        while (i < filterText.Length)
        {
            var c = filterText[i];
            if (currentWordIndex >= wordsSplit.Length)
            {
                return true;
            }

            string currentWord = wordsSplit[currentWordIndex];
            if (currentWord[indexInWord] != c)
            {
                return false;
            }

            if (i == filterText.Length - 1)
            {
                return true;
            }

            var nextCharFilter = filterText[i + 1];
            if (currentWord.Length > indexInWord + 1 && currentWord[indexInWord + 1] == nextCharFilter)
            {
                indexInWord++;
            }
            else
            {
                currentWordIndex++;
                indexInWord = 0;
            }

            i++;
            continue;
        }

        return true;
    }

    static string[] SplitIntoTokens(this string token)
    {
        var currentWord = new List<char>();
        var words = new List<string>();
        foreach (var c in token)
        {
            if (char.IsUpper(c))
            {
                if (currentWord.Count > 0)
                {
                    words.Add(new string(currentWord.ToArray()));
                }

                currentWord.Clear();
                currentWord.Add(char.ToLower(c));
                continue;
            }

            currentWord.Add(c);
        }

        if (currentWord.Count > 0)
        {
            words.Add(new string(currentWord.ToArray()));
        }

        return words.ToArray();
    }
}