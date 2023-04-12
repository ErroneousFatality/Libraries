using PhoneNumbers;

namespace AndrejKrizan.PhoneNumbers.Extensions;

public static class StringExtensions
{
    /// <exception cref="ArgumentException"></exception>
    public static string ToPhoneNumber(this string phoneNumber, PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL, string? regionCode = null, int maxLength = PhoneNumberConstraints.MaxLength)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException($"The phone number must not be empty.", nameof(phoneNumber));
        }
        if (phoneNumber.Length > maxLength)
        {
            throw new ArgumentException($"The phone number \"{phoneNumber}\" is too long. Maximum length: {maxLength}.", nameof(phoneNumber));
        }

        PhoneNumberUtil utils = PhoneNumberUtil.GetInstance();
        PhoneNumber _phoneNumber;
        try
        {
            _phoneNumber = utils.Parse(phoneNumber, regionCode);
        }
        catch (Exception exception)
        {
            throw new ArgumentException($"{CreatePhoneNumberValidationErrorMessage(phoneNumber, regionCode)}: {exception.Message}.", nameof(phoneNumber), exception);
        }
        if (regionCode != null && !utils.IsValidNumberForRegion(_phoneNumber, regionCode))
        {
            throw new ArgumentException(CreatePhoneNumberValidationErrorMessage(phoneNumber, regionCode) + '.', nameof(phoneNumber));
        }
        string validPhoneNumber = utils.Format(_phoneNumber, format);
        return validPhoneNumber;
    }

    private static string CreatePhoneNumberValidationErrorMessage(string phoneNumber, string? regionCode)
    {
        string errorMessage = $"The phone number \"{phoneNumber}\" is not valid";
        if (regionCode != null)
        {
            errorMessage += $" for region \"{regionCode}\"";
        }
        return errorMessage;
    }
}
