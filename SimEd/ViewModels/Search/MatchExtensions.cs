using ZLinq;

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
        if (wordsSplit.Length == 0)
        {
            return false;
        }

        ReadOnlySpan<char> filterSpan = filterText.AsSpan();
        ReadOnlySpan<string> wordsSplitSpan = wordsSplit.AsSpan();
        return FilterViaSpans(filterSpan, wordsSplitSpan);
    }

    private static bool FilterViaSpans(ReadOnlySpan<char> filterSpan, ReadOnlySpan<string> wordsSplitSpan)
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

            filterSpan = filterSpan[positionInWord..];
            wordsSplitSpan = wordsSplitSpan[1..];
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
                words.Add(new string(currentWord.ToArray()));
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

            if (c == '_')
            {
                words.Add(new string(currentWord.ToArray()));
                currentWord.Clear();
                words.Add(new string(currentNumber.ToArray()));
                currentNumber.Clear();
                continue;
            }

            if (char.IsUpper(c))
            {
                words.Add(new string(currentWord.ToArray()));

                currentWord.Clear();
                currentWord.Add(char.ToLower(c));
                continue;
            }

            currentWord.Add(c);
        }

        words.Add(new string(currentWord.ToArray()));
        words.Add(new string(currentNumber.ToArray()));

        return words
            .AsValueEnumerable()
            .Where(w => w.Length > 0)
            .ToArray();
    }
}