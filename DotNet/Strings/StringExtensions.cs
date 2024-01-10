using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Strings;

public static class StringExtensions
{
    public static string ReplaceAt(this string str, int index, char ch)
        => string.Create(str.Length, str, (span, _str) =>
        {
            _str.AsSpan().CopyTo(span);
            span[index] = ch;
        });

    #region SplitToEnumerable
    public static IEnumerable<string> SplitToEnumerable(this string text,
        char separator,
        StringSplitOptions options = StringSplitOptions.None
    )
    {
        bool trim = options.HasFlag(StringSplitOptions.TrimEntries);
        bool ignoreEmpty = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
        for (int start = 0, end; start < text.Length; start = end + 1)
        {
            end = text.IndexOf(separator, start);
            if (end == -1)
            {
                end = text.Length;
            }
            string substring = text[start..end];
            if (trim)
            {
                substring = substring.Trim();
            }
            if (ignoreEmpty && substring.Length == 0)
            {
                continue;
            }
            yield return substring;
        }
    }

    public static IEnumerable<string> SplitToEnumerable(this string text, string separator,
        StringSplitOptions options = StringSplitOptions.None,
        StringComparison comparison = StringComparison.CurrentCulture
    )
    {
        bool trim = options.HasFlag(StringSplitOptions.TrimEntries);
        bool ignoreEmpty = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
        for (int start = 0, end; start < text.Length; start = end + 1)
        {
            end = text.IndexOf(separator, start, comparison);
            if (end == -1)
            {
                end = text.Length;
            }
            string substring = text[start..end];
            if (trim)
            {
                substring = substring.Trim();
            }
            if (ignoreEmpty && substring.Length == 0)
            {
                continue;
            }
            yield return substring;
        }
    }
    #endregion

    #region SplitToPhrases

    public static IEnumerable<string> SplitToPhraseEnumerable(this string text,
        char wordSeparator = ' ',
        char phraseSeparator = '"',
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
    )
    {
        if (wordSeparator == phraseSeparator)
        {
            throw new ArgumentException("The word and phrase separators can not be the same.");
        }

        bool trim = options.HasFlag(StringSplitOptions.TrimEntries);
        bool ignoreEmpty = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);

        FindPhraseBoundaries(text, phraseSeparator, 0, out int phraseStart, out int phraseEnd);
        for (int index = 0; index < text.Length;)
        {
            string phrase;
            if (index == phraseStart)
            {
                phrase = text[(phraseStart + 1)..phraseEnd];
                index = phraseEnd + 1;
                FindPhraseBoundaries(text, phraseSeparator, index, out phraseStart, out phraseEnd);
            }
            else
            {
                int wordBoundary = text.IndexOf(wordSeparator, index);
                if (wordBoundary == -1)
                {
                    wordBoundary = text.Length;
                }
                int wordEnd = Math.Min(wordBoundary, phraseStart);
                phrase = text[index..wordEnd];
                index = wordEnd + (wordEnd == wordBoundary ? 1 : 0);
            }
            if (trim)
            {
                phrase = phrase.Trim();
            }
            if (ignoreEmpty && phrase.Length == 0)
            {
                continue;
            }
            yield return phrase;
        }
    }

    public static ImmutableArray<string> SplitToPhrases(this string text,
        char wordSeparator = ' ',
        char phraseSeparator = '"',
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
    )
        => text.SplitToPhraseEnumerable(wordSeparator, phraseSeparator, options).ToImmutableArray();

    // Private methods
    private static void FindPhraseBoundaries(string text, char phraseSeparator, int startIndex, out int phraseStart, out int phraseEnd)
    {
        phraseStart = text.IndexOf(phraseSeparator, startIndex);
        if (phraseStart == -1)
        {
            phraseStart = text.Length;
            phraseEnd = text.Length;
        }
        else
        {
            phraseEnd = text.IndexOf(phraseSeparator, phraseStart + 1);
            if (phraseEnd == -1)
            {
                phraseStart = text.Length;
                phraseEnd = text.Length;
            }
        }
    }

    #endregion

    public static string? Quote(this string? str)
        => str == null ? str : '\"' + str + '\"';

    #region ToLowercasedFirstCharacter
    public static string ToLowercasedFirstCharacter(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToLower(str[0]));

    public static string ToLowercasedFirstCharacterInvariant(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToLowerInvariant(str[0]));
    #endregion LowercaseFirstCharacter

    #region ToUppercasedFirstCharacter
    public static string ToUppercasedFirstCharacter(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToUpper(str[0]));

    public static string ToUppercasedFirstCharacterInvariant(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToUpperInvariant(str[0]));

    #endregion

    #region EnsureStartsWith
    public static string EnsureStartsWith(this string source, char prefix)
        => source.StartsWith(prefix)
            ? source
            : prefix + source;
    public static string EnsureStartsWith(this string source, string prefix)
        => source.StartsWith(prefix)
            ? source
            : prefix + source;
    #endregion

    #region EnsureEndsWith
    public static string EnsureEndsWith(this string source, char suffix)
        => source.EndsWith(suffix)
            ? source
            : source + suffix;
    public static string EnsureEndsWith(this string source, string suffix)
        => source.EndsWith(suffix)
            ? source
            : source + suffix;
    #endregion

    #region ContainsAny
    // TODO: optimize to run in O(n)
    public static bool ContainsAny(this string source, params string[] strings)
        => strings.Any(source.Contains);

    public static bool ContainsAny(this string source, StringComparison comparison, params string[] strings)
        => strings.Any(needle => source.Contains(needle, comparison));

    public static bool ContainsAny(this string source, params char[] chars)
        => source.IndexOfAny(chars) >= 0;
    #endregion

    #region ReplaceAll
    public static string ReplaceAll(this string source, IEnumerable<char> oldChars, char newChar)
        => source.ReplaceAll(oldChars.ToHashSet(), newChar);

    public static string ReplaceAll(this string source, IReadOnlySet<char> oldChars, char newChar)
    {
        IEnumerable<char> chars = source.Select(character
            => oldChars.Contains(character)
                ? newChar
                : character
        );
        string result = string.Concat(chars);
        return result;
    }

    public static string ReplaceAll(this string source, params (char Old, char New)[] mappings)
    {
        IEnumerable<KeyValuePair<char, char>> keyValuePairs = mappings.Select(mapping => new KeyValuePair<char, char>(mapping.Old, mapping.New));
        Dictionary<char, char> dictionary = new(keyValuePairs);
        string result = source.ReplaceAll(dictionary);
        return result;
    }


    public static string ReplaceAll(this string source, IReadOnlyDictionary<char, char> dictionary)
    {
        IEnumerable<char> chars = source.Select(character
            => dictionary.TryGetValue(character, out char newChar)
                ? newChar
                : character
        );
        string result = string.Concat(chars);
        return result;
    }
    #endregion

    #region RemoveAll
    public static string RemoveAll(this string source, params char[] chars)
    => source.RemoveAll((IEnumerable<char>)chars);

    public static string RemoveAll(this string source, IEnumerable<char> chars)
        => source.RemoveAll(chars.ToHashSet());

    public static string RemoveAll(this string source, IReadOnlySet<char> chars)
    {
        IEnumerable<char> _chars = source.Where(character => !chars.Contains(character));
        string result = string.Concat(_chars);
        return result;
    }
    #endregion
}
