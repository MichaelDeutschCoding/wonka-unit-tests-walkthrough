using Moq;
using Wonka.Customers.Email;
using Wonka.Customers.Validators;
using Wonka.Database;
using Wonka.Models;
using Xunit;

namespace Wonka.Customers.Tests;

public class CustomerManagerTests
{
    private readonly Mock<ICustomersDbAccess> mockDb;
    private readonly Mock<IEmailService> mockEmailService;
    private readonly Mock<ICustomerValidator> mockValidator;

    private readonly CustomerManager sut;

    public CustomerManagerTests()
    {
        mockDb = new();
        mockEmailService = new();
        mockValidator = new();
        sut = new(mockDb.Object, mockEmailService.Object, mockValidator.Object);
    }

    [Fact]
    public void GetAllCustomers_ShouldReturnCustomersFromDatabase()
    {
        // ARRANGE
        var dbCustomers = new List<Customer>();
        mockDb.Setup(db => db.GetCustomers()).Returns(dbCustomers);

        // ACT
        var customers = sut.GetAllCustomers();

        // ASSERT
        Assert.Equal(dbCustomers, customers);
    }

    [Fact]
    public void GetCustomer_CustomerExists_ShouldReturnCustomer()
    {
        // ARRANGE
        var id = 123;
        var dbCustomer = new Customer
        {
            Id = id,
            FirstName = "MyNewGuy",
            LastName = "Silvermansmithbergstein",
            Age = 20,
            EmailAddress = "my_name@gmail.com"
        };
        mockDb.Setup(db => db.GetCustomer(id)).Returns(dbCustomer);

        // ACT
        var customer = sut.GetCustomer(id);

        // ASSERT
        Assert.Equal(dbCustomer, customer);
        mockDb.Verify(db => db.GetCustomer(id), Times.Once);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void GetCustomer_Id_lt_One_ShouldThrow_ArgumentOutOfRangeException(int id)
    {
        // ACT
        Action testCode = () => sut.GetCustomer(id);

        // ASSERT
        var ex = Assert.Throws<ArgumentOutOfRangeException>(testCode);
        Assert.Equal("id", ex.ParamName);
        Assert.Contains("must be greater than 0", ex.Message);
        mockDb.Verify(db => db.GetCustomer(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetCustomer_IdNotInDatabase_ShouldThrow_KeyNotFoundException()
    {
        // ARRANGE
        var id = 10;
        mockDb.Setup(db => db.GetCustomer(It.IsAny<int>())).Returns((Customer?)null);

        // ACT
        void testCode() => sut.GetCustomer(id);

        // ASSERT
        var ex = Assert.Throws<KeyNotFoundException>(testCode);
        Assert.Contains("No customer was found", ex.Message);
        Assert.Contains(id.ToString(), ex.Message);
        mockDb.Verify(db => db.GetCustomer(id), Times.Once);
    }

    [Fact]
    public void AddCustomer_ShouldValidate_AddCustomer_AndSendEmail()
    {
        // ARRANGE
        var email = "mickey_mouse@disney.com";
        var firstName = "Mickey";
        var lastName = "Mouse";
        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = email,
            Age = 84,
        };
        mockDb.Setup(db => db.GetCustomers()).Returns(Enumerable.Empty<Customer>());
        mockDb.Setup(db => db.AddCustomer(customer)).Returns(customer);

        // ACT
        var result = sut.AddCustomer(customer);

        // ASSERT
        Assert.Equal(customer, result);
        mockValidator.Verify(v => v.ValidateCustomerDetails(customer), Times.Once);
        mockValidator.Verify(v => v.ValidateEmailAddress(email, Enumerable.Empty<string>()), Times.Once);
        mockDb.Verify(db => db.AddCustomer(customer), Times.Once);
        mockEmailService.Verify(
            es => es.SendEmail(email, It.Is<string>(body => body.Contains($"{firstName} {lastName}"))),
            Times.Once);
    }

    [Fact]
    public void UpdateCustomer_Exists_ShouldUpdateInDb()
    {
        // ARRANGE
        var id = 10;
        var email = "username@domain.com";
        var customer = new Customer
        {
            Id = id,
            EmailAddress = email,
        };
        mockDb.Setup(db => db.GetCustomers()).Returns(new List<Customer> { customer });
        mockDb.Setup(db => db.GetCustomer(id)).Returns(customer);
        mockDb.Setup(db => db.UpdateCustomer(id, customer)).Returns(customer);

        // ACT
        var result = sut.UpdateCustomer(id, customer);

        // ASSERT
        Assert.Equal(customer, result);
        mockValidator.Verify(v => v.ValidateCustomerDetails(customer), Times.Once);
        mockValidator.Verify(v => v.ValidateEmailAddress(email, Enumerable.Empty<string>()), Times.Once);
        mockDb.Verify(db => db.GetCustomers(), Times.Once);
        mockDb.Verify(db => db.UpdateCustomer(id, customer), Times.Once);
    }
}
