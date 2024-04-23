namespace AndrejKrizan.DotNet.Exceptions;
public class NotFoundException : ArgumentException
{
    // Constructors

    public NotFoundException() { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public NotFoundException(string message, string paramName)
        : base(message, paramName) { }

    public NotFoundException(string message, string paramName, Exception innerException)
        : base(message, paramName, innerException) { }
}
