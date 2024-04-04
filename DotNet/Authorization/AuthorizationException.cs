namespace AndrejKrizan.DotNet.Authorization;

public class AuthorizationException : Exception
{
    // Constructors

    public AuthorizationException() { }

    public AuthorizationException(string message)
        : base(message) { }

    public AuthorizationException(string message, Exception innerException)
        : base(message, innerException) { }
}
