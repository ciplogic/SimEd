using System.Runtime.CompilerServices;
using SimEd.Common.Extensions;

namespace SimEd.Models.Languages.CurlyBasedLanguages;

public class WordsIndex
{
    private readonly char[][] _wordsToMatch;
    private readonly char[] _firstChars;
    private readonly int[] _bitSetFirstChar = new int[8];

    public WordsIndex(string[] wordStrings)
    {
        _wordsToMatch = wordStrings.SelectToArray(w => w.ToCharArray());
        _firstChars = _wordsToMatch.SelectToArray(w => w[0]);
        foreach (var firstChar in _firstChars)
        {
            int charReduced = (firstChar) & 255;
            SetBit(charReduced);
        }
    }

    void SetBit(int position)
    {
        _bitSetFirstChar[position >> 5] |= 1 << (position & 31);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool IsFirstBitSet(char firstChar)
    {
        var position = firstChar & 255;
        var mask = 1 << (position & 31);

        var isBitSet = (_bitSetFirstChar[position >> 5] & mask);
        return isBitSet != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int MatchLen(ArraySegment<char> arraySegment)
    {
        var firstChar = arraySegment[0];
        if (!IsFirstBitSet(firstChar))
        {
            return 0;
        }

        for (var index = 0; index < _firstChars.Length; index++)
        {
            var first = _firstChars[index];
            if (first != firstChar)
            {
                continue;
            }

            var op = _wordsToMatch[index];


            var found = true;
            for (var i = 1; i < op.Length; i++)
            {
                if (op[i] != arraySegment[i])
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