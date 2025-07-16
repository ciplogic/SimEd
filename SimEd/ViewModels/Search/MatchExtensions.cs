namespace SimEd.ViewModels.Search;

internal static class MatchExtensions
{
    public static bool IsSmartMatch(this string token, string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            return true;
        }

        string[] wordsSplit = SplitIntoTokens(token);
        int currentWordIndex = 0;
        int indexInWord = 0;
        int i = 0;
        while (i < filterText.Length)
        {
            char c = filterText[i];
            if (currentWordIndex >= wordsSplit.Length)
            {
                return i + 1 == filterText.Length;;
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

            char nextCharFilter = filterText[i + 1];
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

    private static string[] SplitIntoTokens(this string token)
    {
        List<char> currentWord = new List<char>();
        List<string> words = new List<string>();
        foreach (char c in token)
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