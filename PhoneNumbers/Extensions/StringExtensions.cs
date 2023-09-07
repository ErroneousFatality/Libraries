using System.Text;

using PhoneNumbers;

namespace AndrejKrizan.PhoneNumbers.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="owner">Will be added to the validation error message.</param>
    /// <param name="regionCode">ISO 3166 country alpha-2 code (<see href="https://www.iso.org/obp/ui/#search/code/"/>).</param>
    /// <returns>A region formatted phone number string.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ToPhoneNumber(this string phoneNumber, 
        string? owner = null,
        string? regionCode = null,
        PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL, 
        int maxLength = PhoneNumberConstraints.MaxLength
    )
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException($"{CreateName(owner)} must not be empty.", nameof(phoneNumber));
        }

        if (phoneNumber.Length > maxLength)
        {
            throw new ArgumentException($"{CreateName(owner)} is too long. Maximum length: {maxLength}.", nameof(phoneNumber));
        }

        PhoneNumberUtil utils = PhoneNumberUtil.GetInstance();
        PhoneNumber _phoneNumber;
        try
        {
            _phoneNumber = utils.Parse(phoneNumber, regionCode);
        }
        catch (NumberParseException exception)
        {
            if (exception.ErrorType == ErrorType.INVALID_COUNTRY_CODE)
            {
                throw new ArgumentOutOfRangeException($"The phone number region code \"{regionCode}\" is not valid.", exception);
            }
            throw new ArgumentException(CreateErrorMessage(owner, regionCode), nameof(phoneNumber), exception);
        }

        if (regionCode != null && !utils.IsValidNumberForRegion(_phoneNumber, regionCode))
        {
            throw new ArgumentException(CreateErrorMessage(owner, regionCode), nameof(phoneNumber));
        }

        string validPhoneNumber = utils.Format(_phoneNumber, format);
        return validPhoneNumber;
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
