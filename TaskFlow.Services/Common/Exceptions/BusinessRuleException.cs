// Common/Exceptions/BusinessRuleException.cs
namespace TaskFlow.Services.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}