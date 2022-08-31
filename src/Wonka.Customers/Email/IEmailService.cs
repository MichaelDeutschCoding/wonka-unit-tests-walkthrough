namespace Wonka.Customers.Email;

internal interface IEmailService
{
    void SendEmail(string emailAddress, string body);
    bool IsValidEmailAddress(string emailAddress);
}
