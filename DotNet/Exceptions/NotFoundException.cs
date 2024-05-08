namespace AndrejKrizan.DotNet.Exceptions;
public class NotFoundException : Exception
{
    // Constructors
    public NotFoundException(
        string? message = null, 
        Exception? innerException = null
    )
        : base(message, innerException) { }
}
