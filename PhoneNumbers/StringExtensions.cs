using System.Diagnostics.CodeAnalysis;
using System.Text;

using PhoneNumbers;

using Library = PhoneNumbers;

namespace AndrejKrizan.PhoneNumbers;

public static class StringExtensions
{
    /// <param name="owner">Will be added to the exception messages. E.g.: "The {owner}'s phone number...".</param>
    /// <param name="regionCode">ISO 3166 country alpha-2 code (<see href="https://www.iso.org/obp/ui/#search/code/"/>).</param>
    /// <param name="maxLength">If the resulting string has length greater than this value, then an argument exception will be thrown.</param>
    /// <returns>A valid region formatted phone number derived from this string.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ToPhoneNumber(this string source,
        string? owner = null,
        string? regionCode = null,
        PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL,
        int maxLength = PhoneNumberConstraints.MaxLength
    )
        => source.TryGetPhoneNumber(out string? phoneNumber, out ArgumentException? exception, owner, regionCode, format, maxLength)
            ? phoneNumber
            : throw exception;


    /// <param name="phoneNumber">A valid region formatted phone number derived from this string.</param>
    /// <param name="owner">Will be added to the exception messages. E.g.: "The {owner}'s phone number...".</param>
    /// <param name="regionCode">ISO 3166 country alpha-2 code (<see href="https://www.iso.org/obp/ui/#search/code/"/>).</param>
    /// <param name="maxLength">If the resulting string has length greater than this value, then an argument exception will be thrown.</param>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryGetPhoneNumber(this string source,
        [NotNullWhen(true)] out string? phoneNumber,
        string? owner = null,
        string? regionCode = null,
        PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL,
        int maxLength = PhoneNumberConstraints.MaxLength
    )
        => source.TryGetPhoneNumber(out phoneNumber, out _, owner, regionCode, format, maxLength);

    /// <param name="phoneNumber">A valid region formatted phone number derived from this string.</param>
    /// <param name="owner">Will be added to the exception messages. E.g.: "The {owner}'s phone number...".</param>
    /// <param name="regionCode">ISO 3166 country alpha-2 code (<see href="https://www.iso.org/obp/ui/#search/code/"/>).</param>
    /// <param name="maxLength">If the resulting string has length greater than this value, then an argument exception will be thrown.</param>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryGetPhoneNumber(this string source,
        [NotNullWhen(true)] out string? phoneNumber,
        [NotNullWhen(false)] out ArgumentException? exception,
        string? owner = null,
        string? regionCode = null,
        PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL,
        int maxLength = PhoneNumberConstraints.MaxLength
    )
    {
        if (!Enum.IsDefined(format))
        {
            throw new ArgumentOutOfRangeException(nameof(format));
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            exception = new ArgumentException($"{CreateName(owner)} must not be empty.", nameof(source));
            goto Fail;
        }

        PhoneNumberUtil utils = PhoneNumberUtil.GetInstance();
        PhoneNumber phoneNumberObj;
        try
        {
            phoneNumberObj = utils.Parse(source, regionCode);
        }
        catch (NumberParseException numberParseException)
        {
            exception = (numberParseException.ErrorType == ErrorType.INVALID_COUNTRY_CODE)
                ? new ArgumentOutOfRangeException($"The phone number region code \"{regionCode}\" is not valid.", numberParseException)
                : new ArgumentException(CreateErrorMessage(owner, regionCode), nameof(source), numberParseException);
            goto Fail;
        }

        if (regionCode != null && !utils.IsValidNumberForRegion(phoneNumberObj, regionCode))
        {
            exception = new ArgumentException(CreateErrorMessage(owner, regionCode), nameof(source));
            goto Fail;
        }

        Library.PhoneNumberFormat _format = (Library.PhoneNumberFormat)((int)format - 1);
        phoneNumber = utils.Format(phoneNumberObj, _format);

        if (phoneNumber.Length > maxLength)
        {
            exception = new ArgumentException($"{CreateName(owner)} is too long. Maximum length: {maxLength}.", nameof(source));
            goto Fail;
        }

        exception = null;
        return true;

    Fail:
        phoneNumber = null;
        return false;
    }


    // Private methods
    private static string CreateErrorMessage(string? owner, string? regionCode)
    {
        StringBuilder builder = new(75);
        builder.Append($"{CreateName(owner)} is not valid");
        if (regionCode != null)
        {
            builder.Append($" for region \"{regionCode}\"");
        }
        builder.Append('.');
        return builder.ToString();
    }

    private static string CreateName(string? owner)
    {
        StringBuilder builder = new(41);
        builder.Append("The ");
        if (owner != null)
        {
            builder.Append($"{owner}'s ");
        }
        builder.Append("phone number");
        return builder.ToString();
    }
}
