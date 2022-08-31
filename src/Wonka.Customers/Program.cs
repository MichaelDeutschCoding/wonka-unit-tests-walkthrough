using Wonka.Customers;
using Wonka.Customers.Email;
using Wonka.Database;
using Wonka.Models;


var db = new CustomersDbAccess();
var manager = new CustomerManager(db, new EmailService());

var numberOfCustomers = manager.GetAllCustomers().Count();
Console.WriteLine($"We started off with {numberOfCustomers} customers.");

var mike = new Customer
{
    FirstName = "Mike",
    LastName = "Teavee",
    Age = 25,
    EmailAddress = "mtv@mtv.tv",
};
Console.WriteLine($"\nNow adding {mike.FirstName} as a new customer.");
manager.AddCustomer(mike);

numberOfCustomers = manager.GetAllCustomers().Count();
Console.WriteLine($"Now we have {numberOfCustomers} customers.\n");

var id = 1;
var charlie = manager.GetCustomer(id);
Console.WriteLine($"Now deleting {charlie.FirstName} with ID={id}.\n");
manager.DeleteCustomer(id);
