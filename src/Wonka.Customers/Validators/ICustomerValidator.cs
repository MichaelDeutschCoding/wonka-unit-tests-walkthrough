using Wonka.Models;

namespace Wonka.Customers.Validators;

internal interface ICustomerValidator
{
    void ValidateCustomerDetails(Customer customer);
    void ValidateEmailAddress(string emailAddress, IEnumerable<string> otherEmailAddresses);
}