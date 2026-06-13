using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using TattoStudio.Application.Validation;

namespace TattoStudio.UnitTests.Users.Validation;

public class PasswordStrengthAttributeTests
{
    private readonly PasswordStrengthAttribute _attribute = new();

    private ValidationResult? Validate(string? password)
    {
        var ctx = new ValidationContext(new object());
        return _attribute.GetValidationResult(password, ctx);
    }

    // ── Happy paths ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Secure@pass1")]
    [InlineData("V@lid1234")]
    [InlineData("Minimum8!")]
    [InlineData("ALLCAPS@1")]
    public void IsValid_PasswordMeetsAllRules_ReturnsSuccess(string password)
    {
        var result = Validate(password);
        result.Should().Be(ValidationResult.Success);
    }

    // ── Failing rules ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Sh@rt1")]
    [InlineData("A@1")]
    public void IsValid_TooShort_ReturnsValidationError(string password)
    {
        var result = Validate(password);
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("8 characters");
    }

    [Theory]
    [InlineData("nouppercase@1")]
    [InlineData("secure@pass1")]
    [InlineData("all_lower_123!")]
    public void IsValid_NoUppercase_ReturnsValidationError(string password)
    {
        var result = Validate(password);
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("uppercase");
    }

    [Theory]
    [InlineData("NoSpecial1234")]
    [InlineData("ABCDabcd1234")]
    [InlineData("SecurePass99")]
    public void IsValid_NoSpecialCharacter_ReturnsValidationError(string password)
    {
        var result = Validate(password);
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("special character");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValid_NullOrEmptyPassword_ReturnsRequiredError(string? password)
    {
        var result = Validate(password);
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("required");
    }

    // ── Boundary ───────────────────────────────────────────────────────────────

    [Fact]
    public void IsValid_ExactlyEightCharsWithAllRules_ReturnsSuccess()
    {
        var result = Validate("Minimum8!");
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_SevenCharsOtherwiseValid_ReturnsLengthError()
    {
        var result = Validate("Short@A");
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("8 characters");
    }
}
