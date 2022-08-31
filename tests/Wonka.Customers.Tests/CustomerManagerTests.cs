using Moq;
using Wonka.Customers.Email;
using Wonka.Database;
using Wonka.Models;
using Xunit;

namespace Wonka.Customers.Tests;

public class CustomerManagerTests
{
    private readonly Mock<ICustomersDbAccess> mockDb;
    private readonly Mock<IEmailService> mockEmailService;

    private readonly CustomerManager sut;

    public CustomerManagerTests()
    {
        mockDb = new();
        mockEmailService = new();
        sut = new(mockDb.Object, mockEmailService.Object);
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

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void GetCustomer_Id_lt_One_ShouldThrow_ArgumentOutOfRangeException(int id)
    {
        // ACT
        Action testCode = ()  => sut.GetCustomer(id);

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
}