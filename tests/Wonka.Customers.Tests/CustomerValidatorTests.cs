using Moq;
using Wonka.Customers.Email;
using Wonka.Customers.Validators;
using Wonka.Models;
using Wonka.Models.Exceptions;
using Xunit;

namespace Wonka.Customers.Tests;

public class CustomerValidatorTests
{
    private readonly Mock<IEmailService> mockEmailService;
    private readonly CustomerValidator sut;
    private readonly Customer dummyValidCustomer;

    public CustomerValidatorTests()
    {
        mockEmailService = new();
        dummyValidCustomer = new Customer
        {
            FirstName = "Willy",
            LastName = "Wonka",
            EmailAddress = "willy_wonka@wonkas.com",
            Age = 50,
        };
        sut = new(mockEmailService.Object);
    }
    
    [Theory]
    [MemberData(nameof(InvalidStrings))]
    public void ValidateCustomerDetails_InvalidFirstName_ShouldThrow(string? name)
    {
        // ARRANGE
        dummyValidCustomer.FirstName = name;

        // ACT
        void testDelegate() => sut.ValidateCustomerDetails(dummyValidCustomer);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testDelegate);
        Assert.Contains("FirstName", ex.Message);
    }

    public void ValidateCustomerDetails_InvalidLastName_ShouldThrow(string? name)
    {
        // Write this test
    }

    [Fact]
    public void ValidateCustomerDetails_TooYoung_ShouldThrow()
    {
        // Write this test
    }

    public static IEnumerable<object?[]> InvalidStrings => new List<object?[]>
    {
        new object?[] { null },
        new object[] { "" },
        new object[] { "  " },
        new object[] { "\t\n\t" },
    };
}
