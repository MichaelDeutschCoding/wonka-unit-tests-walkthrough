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
        void testCode() => sut.ValidateCustomerDetails(dummyValidCustomer);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains("FirstName", ex.Message);
    }

    [Theory, MemberData(nameof(InvalidStrings))]
    public void ValidateCustomerDetails_InvalidLastName_ShouldThrow(string? name)
    {
        // ARRANGE
        dummyValidCustomer.LastName = name;

        // ACT
        void testCode() => sut.ValidateCustomerDetails(dummyValidCustomer);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains("LastName", ex.Message);
    }

    [Fact]
    public void ValidateCustomerDetails_TooYoung_ShouldThrow()
    {
        // ARRANGE
        dummyValidCustomer.Age = CustomerValidator.MIN_AGE - 1;

        // ACT
        void testCode() => sut.ValidateCustomerDetails(dummyValidCustomer);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains("must be at least", ex.Message);
        Assert.Contains(CustomerValidator.MIN_AGE.ToString(), ex.Message);
    }

    [Fact]
    public void ValidateCustomerDetails_TooOld_ShouldThrow()
    {
        // ARRANGE
        dummyValidCustomer.Age = CustomerValidator.MAX_AGE + 1;

        // ACT
        void testCode() => sut.ValidateCustomerDetails(dummyValidCustomer);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains("can't actually be older", ex.Message);
        Assert.Contains(CustomerValidator.MAX_AGE.ToString(), ex.Message);
    }

    [Theory]
    [InlineData(18)]
    [InlineData(50)]
    [InlineData(120)]
    public void ValidateCustomerDetails_ValidAge_ShouldPass(int age)
    {
        // ARRANGE
        dummyValidCustomer.Age = age;

        // ACT
        sut.ValidateCustomerDetails(dummyValidCustomer);

        // ASSERT
        Assert.Equal(age, dummyValidCustomer.Age);
    }

    [Fact]
    public void ValidateEmailAddress_Valid_ShouldPass()
    {
        // ARRANGE        
        var address = "some_string"; // NOTE: this doesn't actually have to be a
                                     // valid address einsteinsEmail to pass the test!!
        mockEmailService.Setup(es => es.IsValidEmailAddress(address)).Returns(true);

        // ACT
        sut.ValidateEmailAddress(address, Enumerable.Empty<string>());
    }

    [Fact]
    public void ValidateEmailAddress_InvalidAddress_ShouldThrow()
    {
        // ARRANGE
        var address = "some_string";
        mockEmailService.Setup(es => es.IsValidEmailAddress(address)).Returns(false);

        // ACT
        void testCode() => sut.ValidateEmailAddress(address, Enumerable.Empty<string>());

        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains(address, ex.Message);
        Assert.Contains("not a valid email address", ex.Message);
    }

    [Fact]
    public void ValidateEmailAddress_HasOverlap_ShouldThrow()
    {
        // ARRANGE
        var einsteinsEmail = "a_einstein@gmail.com";
        mockEmailService.Setup(es => es.IsValidEmailAddress(einsteinsEmail)).Returns(true);
        string[] otherAddresses =
        {
            "idan@reichel.com",
            einsteinsEmail,
            "ofra@haza.il"
        };

        // ACT
        void testCode() => sut.ValidateEmailAddress(einsteinsEmail, otherAddresses);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains(einsteinsEmail, ex.Message);
        Assert.Contains("already in use by another customer", ex.Message);
    }

    [Fact]
    public void ValidateEmailAddress_ShouldBeCaseInsensitive()
    {
        // ARRANGE
        var address = "some string";
        mockEmailService.Setup(es => es.IsValidEmailAddress(address)).Returns(true);
        string[] otherAddresses = { address.ToUpper() };

        // ACT
        void testCode() => sut.ValidateEmailAddress(address, otherAddresses);

        // ASSERT
        var ex = Assert.Throws<InvalidCustomerDataException>(testCode);
        Assert.Contains(address, ex.Message);
        Assert.Contains("already in use by another customer", ex.Message);
    }

    public static IEnumerable<object?[]> InvalidStrings => new List<object?[]>
    {
        new object?[] { null },
        new object[] { "" },
        new object[] { "  " },
        new object[] { "\t\n\t" },
    };
}
