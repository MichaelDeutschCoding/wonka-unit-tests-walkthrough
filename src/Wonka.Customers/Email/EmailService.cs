using System.Text.RegularExpressions;

namespace Wonka.Customers.Email;

internal class EmailService : IEmailService
{
    private const string _senderEmailAddress = "no_reply@crossriver.com";
    private static readonly Regex _emailRegex = new(@"^[A-Z0-9._+-]{2,}@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);

    public void SendEmail(string emailAddress, string body)
    {
        Console.WriteLine($"Simulating sending an email from {_senderEmailAddress} to {emailAddress}\n");
        Console.WriteLine(body);
    }

    public bool IsValidEmailAddress(string emailAddress)
    {
        return _emailRegex.IsMatch(emailAddress);
    }
}
