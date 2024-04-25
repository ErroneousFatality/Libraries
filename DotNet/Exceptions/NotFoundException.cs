namespace AndrejKrizan.DotNet.Exceptions;
public class NotFoundException : ArgumentException
{
    // Constructors
    public NotFoundException(
        string? paramName = null, 
        string? message = null, 
        Exception? innerException = null
    )
        : base(message, paramName, innerException) { }
}
