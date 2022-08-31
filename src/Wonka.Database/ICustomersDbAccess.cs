using Wonka.Models;

namespace Wonka.Database;

public interface ICustomersDbAccess
{
    IEnumerable<Customer> GetCustomers();
    Customer? GetCustomer(int id);
    Customer AddCustomer(Customer customer);
    Customer UpdateCustomer(int id, Customer updatedCustomer);
    void DeleteCustomer(Customer customer);
}
