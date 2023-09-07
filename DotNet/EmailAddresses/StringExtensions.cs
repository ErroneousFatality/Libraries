using System.Net.Mail;

namespace AndrejKrizan.DotNet.EmailAddress;

public static class StringExtensions
{
    /// <exception cref="ArgumentException"></exception>
    public static string ToEmailAddress(this string emailAddress, 
        string? owner = null, 
        int maxLength = CommonConstraints.EmailAddressMaxLength
    )
    {
        if (emailAddress.Trim().EndsWith("."))
        {
            throw new ArgumentException($"{CreateName(owner)} must not end with a \'.\'.", nameof(emailAddress));
        }

        string validEmailAddress;
        try
        {
            MailAddress mailAddress = new(emailAddress);
            validEmailAddress = mailAddress.Address;
        }
        catch (Exception exception)
        {
            throw new ArgumentException($"{CreateName(owner)} is not valid: {exception.Message}.", nameof(emailAddress), exception);
        }

        if (validEmailAddress.Length > maxLength)
        {
            throw new ArgumentException($"{CreateName(owner)} is too long. Maximum length: {maxLength}.", nameof(emailAddress));
        }

        return validEmailAddress;
    }

    private static string CreateName(string? owner)
    {
        const string prefix = "The ";
        const string postfix = "email address";

        return owner == null
            ? prefix + postfix
            : $"{prefix}{owner}'s {postfix}";
    }
}
