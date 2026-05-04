using TwilioCallCenter.Common;
using Xunit;

namespace TwilioCallCenter.Tests;

public class PhoneNumberValidatorTests
{
    [Theory]
    [InlineData("+15551234567")]
    [InlineData("+393391234567")]
    [InlineData("+447911123456")]
    public void Accepts_valid_e164_numbers(string number) =>
        Assert.True(PhoneNumberValidator.IsValid(number));

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("15551234567")]
    [InlineData("+0123456789")]
    [InlineData("+12")]
    [InlineData("+1234567890123456")]
    [InlineData("+1-555-123-4567")]
    public void Rejects_invalid_numbers(string? number) =>
        Assert.False(PhoneNumberValidator.IsValid(number));
}
