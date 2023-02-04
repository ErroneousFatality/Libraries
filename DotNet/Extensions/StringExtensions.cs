using System.Net.Mail;

namespace AndrejKrizan.DotNet.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceAt(this string str, int index, char ch)
            => string.Create(str.Length, str, (span, _str) =>
            {
                _str.AsSpan().CopyTo(span);
                span[index] = ch;
            });


        public static IEnumerable<string> SplitToEnumerable(this string str, char separator)
        {
            int startIndex = 0;
            while (startIndex < str.Length)
            {
                int finishIndex = str.IndexOf(separator, startIndex);
                if (finishIndex == -1)
                {
                    finishIndex = str.Length;
                }

                string substring = str[startIndex..finishIndex];
                yield return substring;
                startIndex = finishIndex + 1;
            }
        }

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

        #region AddPrefixIdempotent
        public static string AddPrefixIdempotent(this string str, char prefix)
            => str.StartsWith(prefix)
                ? str
                : prefix + str;
        public static string AddPrefixIdempotent(this string str, string prefix)
            => str.StartsWith(prefix)
                ? str
                : prefix + str;
        #endregion

        #region ContainsAny
        public static bool ContainsAny(this string haystack, params string[] needles)
            => needles.Any(needle => haystack.Contains(needle));

        public static bool ContainsAny(this string haystack, StringComparison comparison, params string[] needles)
            => needles.Any(needle => haystack.Contains(needle, comparison));
        #endregion

        /// <exception cref="ArgumentException"></exception>
        public static string ToEmailAddress(this string emailAddress, int maxLength = CommonConstraints.EmailAddressMaxLength)
        {

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentException($"The email address must not be empty.", nameof(emailAddress));
            }
            if (emailAddress.Trim().EndsWith("."))
            {
                throw new ArgumentException($"The email address must not end with a \'.\'.", nameof(emailAddress));
            }
            string validEmailAddress;
            try
            {
                MailAddress mailAddress = new(emailAddress);
                validEmailAddress = mailAddress.Address;
            }
            catch (Exception exception)
            {
                throw new ArgumentException($"The email address \"{emailAddress}\" is not valid: {exception.Message}.", nameof(emailAddress), exception);
            }

            if (validEmailAddress.Length > maxLength)
            {
                throw new ArgumentException($"The email address \"{validEmailAddress}\" is too long. Maximum length: {maxLength}.", nameof(emailAddress));
            }
            return validEmailAddress;
        }
    }
}
