using Wonka.Models;

namespace Wonka.Database;

public class CustomersDbAccess : ICustomersDbAccess
{
    private static int _idSequence = 1;

    private readonly List<Customer> _customers = new()
    {
        new Customer { FirstName = "Charlie", LastName = "Bucket", Id = _idSequence++, Age = 40, EmailAddress = "c_bucket@gmail.com" },
        new Customer { FirstName = "Veruca", LastName = "Salt", Id = _idSequence++, Age = 25, EmailAddress = "vSalt@yahoo.com" },
        new Customer { FirstName = "Augustus", LastName = "Gloop", Id = _idSequence++, Age = 21, EmailAddress = "a_gloop@duslerdorf.de" },
    };

    public IEnumerable<Customer> GetCustomers()
    {
        return _customers;
    }

    public Customer? GetCustomer(int id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public Customer AddCustomer(Customer customer)
    {
        customer.Id = _idSequence++;
        _customers.Add(customer);

        return customer;
    }

    public Customer UpdateCustomer(int id, Customer updatedCustomer)
    {
        var customer = _customers.First(c => c.Id == id);

        customer.FirstName = updatedCustomer.FirstName;
        customer.LastName = updatedCustomer.LastName;
        customer.EmailAddress = updatedCustomer.EmailAddress;
        customer.Age = updatedCustomer.Age;

        return customer;
    }

    public void DeleteCustomer(Customer customer)
    {
        _ = _customers.Remove(customer);
    }
}
