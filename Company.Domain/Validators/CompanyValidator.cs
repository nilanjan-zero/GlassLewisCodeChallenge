using FluentValidation;
using Company.Domain.Models;

namespace Company.Domain.Validators
{
    public class CompanyValidator : AbstractValidator<Company.Domain.Models.Company>
    {
        public CompanyValidator()
        {
            RuleFor(company => company.ISIN)
                .NotEmpty().WithMessage("ISIN is required.")
                .Matches("^[A-Za-z]{2}[A-Za-z0-9]*$").WithMessage("The first two characters of an ISIN must be letters / non numeric");

            RuleFor(company => company.Website)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("Website must be a valid URL.");
        }
    }
}
