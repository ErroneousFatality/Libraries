using PhoneNumbers;

namespace AndrejKrizan.PhoneValidation.Extensions
{
    public static class StringExtensions
    {
        /// <exception cref="ArgumentException"></exception>
        public static string ToPhoneNumber(this string phoneNumber, PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL, string? regionCode = null, int maxLength = CommonConstraints.PhoneNumberMaxLength)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException($"The phone number must not be empty.", nameof(phoneNumber));
            }
            if (phoneNumber.Length > maxLength)
            {
                throw new ArgumentException($"The phone number \"{phoneNumber}\" is too long. Maximum length: {maxLength}.", nameof(phoneNumber));
            }

            string errorMessage = $"The phone number \"{phoneNumber}\" is not valid";
            if (regionCode != null) {
                errorMessage += $" for region \"{regionCode}\"";
            }

            PhoneNumberUtil utils = PhoneNumberUtil.GetInstance();
            PhoneNumber _phoneNumber;
            try
            {
                _phoneNumber = utils.Parse(phoneNumber, regionCode);
            }
            catch (Exception exception)
            {
                throw new ArgumentException($"{errorMessage}: {exception.Message}.", nameof(phoneNumber), exception);
            }
            if (regionCode != null && !utils.IsValidNumberForRegion(_phoneNumber, regionCode))
            {
                throw new ArgumentException($"{errorMessage}.", nameof(phoneNumber));
            }
            string validPhoneNumber = utils.Format(_phoneNumber, format);
            return validPhoneNumber;
        }
    }
}
