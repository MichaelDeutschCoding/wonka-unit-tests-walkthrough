using Wonka.Customers.Email;
using Xunit;

namespace Wonka.Customers.Tests;

public  class EmailServiceTests
{
    private readonly EmailService sut = new();

    [Theory]
    [InlineData("john_smith@google.com")]
    [InlineData("JOHN_SMITH@GOOGLE.COM")]
    [InlineData("js@a.zz")]
    [InlineData("js@google.co")]
    [InlineData("js@google.com")]
    [InlineData("js@google.coms")]
    [InlineData("john_smith-123.45+67@google.com")]
    [InlineData("john_smith@google123.com")]
    [InlineData("john_smith@google-123.com")]
    public void IsValidEmailAddress_Valid_ShouldReturn_True(string emailAddress)
    {
        // ACT
        var result = sut.IsValidEmailAddress(emailAddress);

        // ASSERT
        Assert.True(result);
    }

    [Theory]
    [InlineData("j@google.com")]
    [InlineData("john smith@google.com")]
    [InlineData("john smith@google_123.com")]
    [InlineData("john_smith@google.loong")]
    [InlineData("יוחנן_סמיט@google.com")]
    public void IsValidEmailAddress_Invalid_ShouldReturn_False(string emailAddress)
    {
        // ACT
        var result = sut.IsValidEmailAddress(emailAddress);

        // ASSERT
        Assert.False(result);
    }
}
