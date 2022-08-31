using Wonka.Models;

namespace Wonka.Customers;

internal interface ICustomerManager
{
    IEnumerable<Customer> GetAllCustomers();
    Customer GetCustomer(int id);
    Customer AddCustomer(Customer customer);
    Customer UpdateCustomer(int id, Customer customer);
    void DeleteCustomer(int id);
}
