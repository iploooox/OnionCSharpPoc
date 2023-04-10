using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using OnionCSharpPoc.Infrastructure.Extensions;

namespace OnionCSharpPocTest.Infrastructure.Extensions;

public class UnitTestValidationExtensions
{
    [Fact]
    public void ToJsonString_ReturnsExpectedJsonString_ForValidResult()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var jsonString = validationResult.ToJsonString();

        // Assert
        jsonString.Should().Be("{\"errors\":[]}");
    }

    [Fact]
    public void ToJsonString_ReturnsExpectedJsonString_ForInvalidResult()
    {
        // Arrange
        var validationFailure = new ValidationFailure("Name", "Name is required");
        var validationResult = new ValidationResult(new[] { validationFailure });

        // Act
        var jsonString = validationResult.ToJsonString();

        // Assert
        jsonString.Should().Be("{\"errors\":[{\"PropertyName\":\"Name\",\"ErrorMessage\":\"Name is required\"}]}");
    }

    [Fact]
    public void ToJsonString_ReturnsExpectedJsonString_ForMultipleValidationFailures()
    {
        // Arrange
        var validationFailure1 = new ValidationFailure("Name", "Name is required");
        var validationFailure2 = new ValidationFailure("Email", "Email is not valid");
        var validationResult = new ValidationResult(new[] { validationFailure1, validationFailure2 });

        // Act
        var jsonString = validationResult.ToJsonString();

        // Assert
        jsonString.Should().Be("{\"errors\":[{\"PropertyName\":\"Name\",\"ErrorMessage\":\"Name is required\"},{\"PropertyName\":\"Email\",\"ErrorMessage\":\"Email is not valid\"}]}");
    }
}

public class UnitTestValidationExceptionExtensionsTests
{
    [Fact]
    public void ToJsonString_ReturnsExpectedJsonString_ForValidationException()
    {
        // Arrange
        var validationFailure1 = new ValidationFailure("Name", "Name is required");
        var validationFailure2 = new ValidationFailure("Email", "Email is not valid");
        var validationResult = new ValidationResult(new[] { validationFailure1, validationFailure2 });
        var validationException = new ValidationException(validationResult.Errors);

        // Act
        var jsonString = validationException.ToJsonString();

        // Assert
        jsonString.Should().Be("{\"errors\":[{\"PropertyName\":\"Name\",\"ErrorMessage\":\"Name is required\"},{\"PropertyName\":\"Email\",\"ErrorMessage\":\"Email is not valid\"}]}");
    }

    [Fact]
    public void ToProblemDetails_ReturnsExpectedHttpValidationProblemDetails_ForValidationException()
    {
        // Arrange
        var validationFailure1 = new ValidationFailure("Name", "Name is required");
        var validationFailure2 = new ValidationFailure("Email", "Email is not valid");
        var validationResult = new ValidationResult(new[] { validationFailure1, validationFailure2 });
        var validationException = new ValidationException(validationResult.Errors);

        // Act
        var problemDetails = validationException.ToProblemDetails();

        // Assert
        problemDetails.Errors.Should().HaveCount(2);
        problemDetails.Errors.Should().ContainKey("Name");
        problemDetails.Errors["Name"].Should().BeEquivalentTo("Name is required");
        problemDetails.Errors.Should().ContainKey("Email");
        problemDetails.Errors["Email"].Should().BeEquivalentTo("Email is not valid");
    }
}
