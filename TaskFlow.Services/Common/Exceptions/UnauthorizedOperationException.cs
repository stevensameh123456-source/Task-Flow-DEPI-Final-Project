// Common/Exceptions/UnauthorizedOperationException.cs
namespace TaskFlow.Services.Common.Exceptions;

public class UnauthorizedOperationException : Exception
{
    public UnauthorizedOperationException(string message = "You are not allowed to perform this action.")
        : base(message) { }
}