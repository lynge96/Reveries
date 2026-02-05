using FluentValidation;
using Reveries.Contracts.Books;

namespace Reveries.Api.Validation;

public class CreateBookDtoValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Book title is required.");
        
        RuleFor(x => x.Isbn13)
            .Matches(@"^\d{13}$")
            .When(x => !string.IsNullOrEmpty(x.Isbn13))
            .WithMessage("ISBN-13 must be 13 digits if provided.");
        
        RuleFor(x => x.Isbn10)
            .Matches(@"^\d{10}$")
            .When(x => !string.IsNullOrEmpty(x.Isbn10))
            .WithMessage("ISBN-10 must be 10 digits if provided.");

        RuleFor(x => x.Pages)
            .GreaterThan(0)
            .When(x => x.Pages.HasValue)
            .WithMessage("Page count must be greater than zero.");
    }
}