using FluentValidation;
using Company.Domain.ViewModels;

namespace Company.Domain.Validators
{
    public class CompanyValidator : AbstractValidator<CompanyVM>
    {
        public CompanyValidator()
        {
            RuleFor(company => company.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(company => company.ISIN)
                .NotEmpty().WithMessage("ISIN is required.")
                .Matches("^[A-Za-z]{2}[A-Za-z0-9]*$").WithMessage("The first two characters of an ISIN must be letters / non numeric");

            RuleFor(company => company.Website)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("Website must be a valid URL.");
        }
    }
}
