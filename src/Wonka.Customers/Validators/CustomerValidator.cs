using Wonka.Customers.Email;
using Wonka.Models.Exceptions;
using Wonka.Models;

namespace Wonka.Customers.Validators;

internal class CustomerValidator : ICustomerValidator
{
    public const int MIN_AGE = 18;
    public const int MAX_AGE = 120;

    private readonly IEmailService _emailService;

    public CustomerValidator(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void ValidateCustomerDetails(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FirstName))
            throw new InvalidCustomerDataException($"Customer must provide a {nameof(customer.FirstName)}.");

        if (string.IsNullOrWhiteSpace(customer.LastName))
            throw new InvalidCustomerDataException($"Customer must provide a {nameof(customer.LastName)}.");

        if (customer.Age < MIN_AGE)
            throw new InvalidCustomerDataException($"Customer must be at least {MIN_AGE} years old to register.");

        if (customer.Age > MAX_AGE)
            throw new InvalidCustomerDataException($"You can't actually be older than {MAX_AGE}! Please set your age to a more believable number ☺");
    }

    public void ValidateEmailAddress(string emailAddress, IEnumerable<string> otherEmailAddreses)
    {
        if (!_emailService.IsValidEmailAddress(emailAddress))
            throw new InvalidCustomerDataException($"'{emailAddress}' is not a valid email address.");

        bool hasOverlap = otherEmailAddreses
            .Any(otherEmail => otherEmail.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

        if (hasOverlap)
            throw new InvalidCustomerDataException($"The provided email address '{emailAddress}' is already in use by another customer.");
    }
}
