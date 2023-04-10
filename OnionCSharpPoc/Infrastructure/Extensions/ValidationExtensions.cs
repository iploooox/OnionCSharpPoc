using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;

namespace OnionCSharpPoc.Infrastructure.Extensions;

public static class ValidationExtensions
{
    public static string ToJsonString(this ValidationResult validationResult)
    {
        return validationResult.Errors.ToJsonString();
    }
    
    public static string ToJsonString(this ValidationException validationException)
    {
        return validationException.Errors.ToJsonString();
    }

    public static string ToJsonString(this IEnumerable<ValidationFailure> validationFailures)
    {
        var errors = validationFailures.Select(x => new { x.PropertyName, x.ErrorMessage });
        return JsonSerializer.Serialize(new { errors });
    }

    public static HttpValidationProblemDetails ToProblemDetails(this ValidationException validationException)
    {
        return validationException.Errors.ToProblemDetails();
    }

    private static HttpValidationProblemDetails ToProblemDetails(this IEnumerable<ValidationFailure> validationFailures)
    {
        var errorsGrouped = validationFailures.GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        return new HttpValidationProblemDetails(errorsGrouped);
    }
}