using System.Collections;
using System.Runtime.CompilerServices;
using SimEd.Common.Extensions;

namespace SimEd.Models.Languages.CurlyBasedLanguages;

public class WordsIndex
{
    private readonly BitArray _bitArray;
    private readonly char[] _firstChars;
    private readonly char[][] _wordsToMatch;

    public WordsIndex(string[] wordStrings)
    {
        _wordsToMatch = wordStrings.Order().ToArray().SelectToArray(w => w.ToCharArray());
        _firstChars = _wordsToMatch.SelectToArray(w => w[0]);
        _bitArray = new BitArray(new byte[32]);
        foreach (var c in _firstChars)
        {
            _bitArray.Set(c, true);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int MatchLen(ArraySegment<char> text)
    {
        char firstChar = text[0];
        if (firstChar >= 256)
        {
            return 0;
        }

        if (!_bitArray[firstChar])
        {
            return 0;
        }

        for (int index = 0; index < _firstChars.Length; index++)
        {
            char first = _firstChars[index];
            if (first != firstChar)
            {
                continue;
            }

            char[] op = _wordsToMatch[index];
            if (op.Length > text.Count)
            {
                continue;
            }

            bool found = true;
            for (int i = 1; i < op.Length; i++)
            {
                if (op[i] != text[i])
                {
                    found = false;
                    break;
                }
            }

            if (found)
            {
                return op.Length;
            }
        }

        return 0;
    }
}