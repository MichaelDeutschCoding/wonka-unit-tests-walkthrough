using Wonka.Customers.Email;
using Wonka.Customers.Validators;
using Wonka.Database;
using Wonka.Models;

namespace Wonka.Customers;

internal class CustomerManager : ICustomerManager
{
    private readonly ICustomersDbAccess _customersDbAccess;
    private readonly IEmailService _emailService;
    private readonly ICustomerValidator _validator;

    public CustomerManager(ICustomersDbAccess customersDbAccess, IEmailService emailService, ICustomerValidator validator)
    {
        _customersDbAccess = customersDbAccess;
        _emailService = emailService;
        _validator = validator;
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
        _validator.ValidateCustomerDetails(customer);
        var otherEmails = _customersDbAccess.GetCustomers()
            .Select(cust => cust.EmailAddress);
        _validator.ValidateEmailAddress(customer.EmailAddress, otherEmails);

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
        _validator.ValidateCustomerDetails(customer);
        var otherEmails = _customersDbAccess.GetCustomers()
            .Select(cust => cust.EmailAddress);
        _validator.ValidateEmailAddress(customer.EmailAddress, otherEmails);

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
}
