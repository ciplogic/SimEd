using System.Runtime.CompilerServices;
using SimEd.Common.Extensions;

namespace SimEd.Models.Languages.CurlyBasedLanguages;

public class WordsIndex
{
    private readonly char[] _firstChars;
    private readonly char[][] _wordsToMatch;
    
    public WordsIndex(string[] wordStrings)
    {
        _wordsToMatch = wordStrings.Order().ToArray().SelectToArray(w => w.ToCharArray());
        _firstChars = _wordsToMatch.SelectToArray(w => w[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int MatchLen(ArraySegment<char> arraySegment)
    {
        char firstChar = arraySegment[0];

        for (int index = 0; index < _firstChars.Length; index++)
        {
            char first = _firstChars[index];
            if (first != firstChar)
            {
                continue;
            }

            char[] op = _wordsToMatch[index];


            bool found = true;
            for (int i = 1; i < op.Length; i++)
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