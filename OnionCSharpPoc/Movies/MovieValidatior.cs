using FluentValidation;

namespace OnionCSharpPoc.Movies;

public class MovieValidator : AbstractValidator<MovieEntity>
{
    public MovieValidator()
    {
        RuleFor(m => m.Title).NotEmpty().MaximumLength(100);
        RuleFor(m => m.Director).NotEmpty().MaximumLength(100);
        RuleFor(m => m.ReleaseYear).InclusiveBetween(1900, DateTime.UtcNow.Year);
    }
}
