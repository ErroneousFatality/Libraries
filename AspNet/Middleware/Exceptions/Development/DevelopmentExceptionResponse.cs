namespace AndrejKrizan.AspNet.Middleware.Exceptions.Development;

public class DevelopmentExceptionResponse
{
    // Properties
    public string Message { get; }
    public string? StackTrace { get; }

    // Constructors
    internal DevelopmentExceptionResponse(Exception exception)
    {
        Message = exception.Message;
        StackTrace = exception.StackTrace;
    }
}
