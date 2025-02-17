using FluentValidation.TestHelper;
using Company.Domain.Validators;
using Xunit;
using Company.Domain.ViewModels;

namespace Company.UnitTests
{
    public class CompanyValidatorTests
    {
        private readonly CompanyValidator _validator;

        public CompanyValidatorTests()
        {
            _validator = new CompanyValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ISIN_Is_Empty()
        {
            var company = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "CA", ISIN = "", Website = "http://companya.com" };
            var result = _validator.TestValidate(company);
            result.ShouldHaveValidationErrorFor(c => c.ISIN).WithErrorMessage("ISIN is required.");
        }

        [Fact]
        public void Should_Have_Error_When_ISIN_Is_Invalid()
        {
            var company = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "CA", ISIN = "123456", Website = "http://companya.com" };
            var result = _validator.TestValidate(company);
            result.ShouldHaveValidationErrorFor(c => c.ISIN).WithErrorMessage("The first two characters of an ISIN must be letters / non numeric");
        }

        [Fact]
        public void Should_Not_Have_Error_When_ISIN_Is_Valid()
        {
            var company = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "CA", ISIN = "US1234567", Website = "http://companya.com" };
            var result = _validator.TestValidate(company);
            result.ShouldNotHaveValidationErrorFor(c => c.ISIN);
        }

        [Fact]
        public void Should_Have_Error_When_Website_Is_Invalid()
        {
            var company = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "CA", ISIN = "", Website = "abc" };
            var result = _validator.TestValidate(company);
            result.ShouldHaveValidationErrorFor(c => c.Website).WithErrorMessage("Website must be a valid URL.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Website_Is_Valid()
        {
            var company = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "CA", ISIN = "US1234567", Website = "http://companya.com" };
            var result = _validator.TestValidate(company);
            result.ShouldNotHaveValidationErrorFor(c => c.Website);
        }
    }
}
