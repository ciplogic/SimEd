using System.Runtime.CompilerServices;

namespace SimEd.Models.Languages.CurlyBasedLanguages;

public static class CurlyLexerRules
{
    public static int CommentMatch(ArraySegment<char> text)
    {
        if (text.Count < 2 || text[0] != '/')
        {
            return 0;
        }

        switch (text[1])
        {
            case '/':
            {
                for (int i = 2; i < text.Count; i++)
                {
                    if (text[i] == '\n' || text[i] == '\r')
                    {
                        return i + 1;
                    }
                }

                return text.Count;
            }
            case '*':
            {
                for (int i = 2; i < text.Count - 1; i++)
                {
                    if (text[i] == '*' && text[i + 1] == '/')
                    {
                        return i + 2;
                    }
                }


                return text.Count;
            }
        }

        return 0;
    }

    public static int StringMatch(ArraySegment<char> arg)
    {
        if (arg[0] != '"' && arg[0] != '\'')
        {
            return 0;
        }

        int i = 1;
        while (i < arg.Count)
        {
            if (arg[i] == '\\')
            {
                i += 2;
                continue;
            }

            if (arg[i] == arg[0])
            {
                return i + 1;
            }

            i++;
        }

        return arg.Count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int SpacesMatch(ArraySegment<char> segment)
    {
        for (var index = 0; index < segment.Count; index++)
        {
            var c = segment[index];
            bool result = c == ' ' || c == '\t';
            if (!result)
            {
                return index;
            }
        }

        return segment.Count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int EolnMatch(ArraySegment<char> segment)
    {
        for (var index = 0; index < segment.Count; index++)
        {
            var c = segment[index];
            bool result = c == '\n' || c == '\r';
            if (!result)
            {
                return index;
            }

        }

        return segment.Count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int IdentifierMatch(ArraySegment<char> segment)
    {
        if (!IsMatchStartForIdentifier(segment[0]))
        {
            return 0;
        }

        segment = segment[1..];
        int pos = 1;
        for (var index = 0; index < segment.Count; index++)
        {
            var b = segment[index];
            if (!IsMatchForIdentifier(b))
            {
                return pos;
            }

            pos++;
        }

        return segment.Count;
    }

    private static bool IsMatchStartForIdentifier(char c)
        => Char.IsLetter(c) || c == '_';

    private static bool IsMatchForIdentifier(char c)
        => IsMatchStartForIdentifier(c) || Char.IsDigit(c);

    public static int MatchArrayOfWordsLength(ArraySegment<char> arg, WordsIndex wordsIndex) 
        => wordsIndex.MatchLen(arg);
    
    
    public static int NumberMatch(ArraySegment<char> segment)
    {
        if (!Char.IsDigit(segment[0]))
        {
            return 0;
        }

        for (var index = 0; index < segment.Count; index++)
        {
            var b = segment[index];
            bool result = Char.IsDigit(b);
            if (!result)
            {
                return index;
            }
        }

        return segment.Count;
    }
}