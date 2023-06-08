namespace Exceptions
{
    public class ExceptionHandler : Exception
    {
        public ExceptionHandler(string message) : base(message)
        {
            
        }
    }
    public class ConflictException : ExceptionHandler
    {
        public ConflictException(string message) : base(message)
        {

        }
    }
    public class ForbiddenException : ExceptionHandler
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }
    public class NoContentException : ExceptionHandler
    {
        public NoContentException(string message) : base(message)
        {

        }
    }
    public class NotFoundException : ExceptionHandler
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
    public class UnauthorizedException : ExceptionHandler
    {
        public UnauthorizedException(string message) : base(message)
        {

        }
    }
    public class BadRequestException : ExceptionHandler
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }
}