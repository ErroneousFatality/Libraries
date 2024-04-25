namespace AndrejKrizan.DotNet.Exceptions;

public class AuthorizationException : InvalidOperationException
{
    // Constructors
    public AuthorizationException() { }

    public AuthorizationException(string message)
        : base(message) { }

    public AuthorizationException(Exception innerException)
        : base(null, innerException) { }

    public AuthorizationException(string message, Exception innerException)
        : base(message, innerException) { }
}
