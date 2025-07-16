namespace SimEd.ViewModels.Search;

public static class MatchExtensions
{
    public static bool IsSmartMatch(this string token, string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            return true;
        }

        string[] wordsSplit = SplitIntoTokens(token);
        ReadOnlySpan<char> filterSpan = filterText.AsSpan();
        Span<string> wordsSplitSpan = wordsSplit.AsSpan();
        return FilterViaSpans(filterSpan, wordsSplitSpan);
    }

    private static bool FilterViaSpans(ReadOnlySpan<char> filterSpan, Span<string> wordsSplitSpan)
    {
        while (filterSpan.Length > 0)
        {
            string currentWord = wordsSplitSpan[0];
            int minWords = Math.Min(filterSpan.Length, currentWord.Length);
            int positionInWord = 0;
            while (positionInWord < minWords && currentWord[positionInWord] == filterSpan[positionInWord])
            {
                positionInWord++;
            }

            if (positionInWord == 0)
            {
                return false;
            }

            filterSpan = filterSpan.Slice(positionInWord);
            wordsSplitSpan = wordsSplitSpan.Slice(1);
            if (wordsSplitSpan.Length == 0)
            {
                return filterSpan.Length == 0;
            }
        }

        return true;
    }

    private static string[] SplitIntoTokens(this string token)
    {
        List<char> currentWord = [];
        List<char> currentNumber = [];
        List<string> words = [];
        for (var index = 0; index < token.Length; index++)
        {
            var c = token[index];
            if (char.IsDigit(c))
            {
                if (currentWord.Count > 0)
                {
                    words.Add(new string(currentWord.ToArray()));
                }

                currentWord.Clear();
                currentNumber.Add(c);
                continue;
            }

            if (currentNumber.Count > 0)
            {
                if (char.IsDigit(c))
                {
                    currentNumber.Add(c);
                    continue;
                }

                words.Add(new string(currentNumber.ToArray()));
                currentNumber.Clear();
            }


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

        if (currentNumber.Count > 0)
        {
            words.Add(new string(currentNumber.ToArray()));
        }

        return words.ToArray();
    }
}