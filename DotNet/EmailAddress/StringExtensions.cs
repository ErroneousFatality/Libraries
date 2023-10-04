using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace AndrejKrizan.DotNet.EmailAddress;

public static class StringExtensions
{
    /// <param name="owner">Will be added to the exception messages. E.g.: "The {owner}'s email address...".</param>
    /// <param name="maxLength">If the resulting string has length greater than this value, then an argument exception will be thrown.</param>
    /// <returns>A valid and formatted email address derived from this string.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ToEmailAddress(this string source,
        string? owner = null,
        int maxLength = CommonConstraints.EmailAddressMaxLength
    )
        => source.TryGetEmailAddress(out string? emailAddress, out ArgumentException? exception, owner, maxLength)
            ? emailAddress
            : throw exception;


    /// <param name="emailAddress">A valid and formatted email address derived from this string.</param>
    /// <param name="owner">Will be added to the exception messages. E.g.: "The {owner}'s email address...".</param>
    /// <param name="maxLength">If the resulting string has length greater than this value, then an argument exception will be thrown.</param>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryGetEmailAddress(this string source,
        [NotNullWhen(true)] out string? emailAddress,
        [NotNullWhen(false)] out ArgumentException? exception,
        string? owner = null,
        int maxLength = CommonConstraints.EmailAddressMaxLength
    )
    {
        if (source.Trim().EndsWith("."))
        {
            exception = new ArgumentException($"{CreateName(owner)} must not end with a \'.\'.", nameof(source));
            goto Fail;
        }

        try
        {
            MailAddress mailAddress = new(source);
            emailAddress = mailAddress.Address;
        }
        catch (Exception _exception)
        {
            exception = new ArgumentException($"{CreateName(owner)} is not valid: {_exception.Message}.", nameof(source), _exception);
            goto Fail;
        }

        if (emailAddress.Length > maxLength)
        {
            exception = new ArgumentException($"{CreateName(owner)} is too long. Maximum length: {maxLength}.", nameof(source));
            goto Fail;
        }

        exception = null;
        return true;

    Fail:
        emailAddress = null;
        return false;
    }

    // Private methods
    private static string CreateName(string? owner)
    {
        const string prefix = "The ";
        const string postfix = "email address";

        return owner == null
            ? prefix + postfix
            : $"{prefix}{owner}'s {postfix}";
    }
}
