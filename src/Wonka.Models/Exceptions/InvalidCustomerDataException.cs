namespace Wonka.Models.Exceptions;

public class InvalidCustomerDataException : Exception
{
    public InvalidCustomerDataException(string? message)
        : base(message) { }
}
