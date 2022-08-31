using Wonka.Customers.Email;
using Wonka.Database;
using Wonka.Models;
using Wonka.Models.Exceptions;

namespace Wonka.Customers;

internal class CustomerManager : ICustomerManager
{
    public const int MIN_AGE = 18;
    public const int MAX_AGE = 120;

    private readonly ICustomersDbAccess _customersDbAccess;
    private readonly IEmailService _emailService;

    public CustomerManager(ICustomersDbAccess customersDbAccess, IEmailService emailService)
    {
        _customersDbAccess = customersDbAccess;
        _emailService = emailService;
    }

    public IEnumerable<Customer> GetAllCustomers()
    {
        var customers = _customersDbAccess.GetCustomers();

        Console.WriteLine($"Fetched {customers.Count()} customers from the Database");
        return customers;
    }

    public Customer GetCustomer(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");

        var customer = _customersDbAccess.GetCustomer(id);
        if (customer is null)
            throw new KeyNotFoundException($"No customer was found with ID: {id}.");

        Console.WriteLine($"Returning customer with ID: {id}");
        return customer;
    }

    public Customer AddCustomer(Customer customer)
    {
        ValidateCustomerDetails(customer);
        var otherEmails = _customersDbAccess.GetCustomers()
            .Select(cust => cust.EmailAddress);
        VerifyNoOverlappingEmail(customer.EmailAddress, otherEmails);

        customer = _customersDbAccess.AddCustomer(customer);

        var emailBody =
            $"To: {customer.FirstName} {customer.LastName} ({customer.EmailAddress})\n\n" +
            $"Dear {customer.FirstName},\n\t" +
            $"Thank you so much for joining us here at Wonka's!\n";
        _emailService.SendEmail(customer.EmailAddress, emailBody);

        Console.WriteLine($"Added a new customer with ID: {customer.Id}.");
        return customer;
    }

    public Customer UpdateCustomer(int id, Customer customer)
    {
        ValidateCustomerDetails(customer);

        var existingCustomer = _customersDbAccess.GetCustomer(id);

        if (existingCustomer is null)
        {
            throw new KeyNotFoundException($"No customer was found to update with ID: {id}.");
        }

        Console.WriteLine($"Updating customer with ID: {id}.");
        return _customersDbAccess.UpdateCustomer(id, customer);
    }

    public void DeleteCustomer(int id)
    {
        var customer = _customersDbAccess.GetCustomer(id);

        if (customer is null)
        {
            throw new KeyNotFoundException($"No customer was found to delete with ID: {id}.");
        }

        var emailBody = $"Dear {customer.FirstName},\n\n\tWe're so sorry to see you leave.";
        _emailService.SendEmail(emailBody, customer.EmailAddress);
        _customersDbAccess.DeleteCustomer(customer);

        Console.WriteLine($"Deleted customer with ID: {id}.");
    }

    private void ValidateCustomerDetails(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FirstName))
            throw new InvalidCustomerDataException($"Customer must provide a {nameof(customer.FirstName)}.");

        if (string.IsNullOrWhiteSpace(customer.LastName))
            throw new InvalidCustomerDataException($"Customer must provide a {nameof(customer.FirstName)}.");

        if (customer.Age <= MIN_AGE)
            throw new InvalidCustomerDataException($"Customer must be at least {MIN_AGE} years old to register.");

        if (customer.Age > MAX_AGE)
            throw new InvalidCustomerDataException($"You can't actually be older than {MAX_AGE}! Please set your age to a more believable number ☺");

        if (_emailService.IsValidEmailAddress(customer.EmailAddress))
            throw new InvalidCustomerDataException($"'{customer.EmailAddress}' is not a valid email address.");
    }

    private static void VerifyNoOverlappingEmail(string email, IEnumerable<string> otherEmailAddreses)
    {
        bool hasOverlap = otherEmailAddreses
            .Any(otherEmail => otherEmail.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (hasOverlap)
            throw new InvalidCustomerDataException($"The provided email address '{email}' is already in use by another customer.");
    }
}
