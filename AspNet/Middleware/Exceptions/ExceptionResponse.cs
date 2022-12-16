namespace AndrejKrizan.AspNet.Middleware.Exceptions
{
    public class ExceptionResponse
    {
        // Properties
        public string Message { get; }

        // Constructors
        internal ExceptionResponse(Exception exception)
        {
            Message = exception.Message;
        }
    }
}
