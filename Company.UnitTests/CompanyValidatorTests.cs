using FluentValidation.TestHelper;
using Company.Domain.Models;
using Company.Domain.Validators;
using Xunit;

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
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "", "http://companya.com", DateTime.Now, DateTime.Now);
            var result = _validator.TestValidate(company);
            result.ShouldHaveValidationErrorFor(c => c.ISIN).WithErrorMessage("ISIN is required.");
        }

        [Fact]
        public void Should_Have_Error_When_ISIN_Is_Invalid()
        {
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "123456", "http://companya.com", DateTime.Now, DateTime.Now);
            var result = _validator.TestValidate(company);
            result.ShouldHaveValidationErrorFor(c => c.ISIN).WithErrorMessage("The first two characters of an ISIN must be letters / non numeric");
        }

        [Fact]
        public void Should_Not_Have_Error_When_ISIN_Is_Valid()
        {
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567", "http://companya.com", DateTime.Now, DateTime.Now);
            var result = _validator.TestValidate(company);
            result.ShouldNotHaveValidationErrorFor(c => c.ISIN);
        }

        [Fact]
        public void Should_Have_Error_When_Website_Is_Invalid()
        {
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "", "abc", DateTime.Now, DateTime.Now);
            var result = _validator.TestValidate(company);
            result.ShouldHaveValidationErrorFor(c => c.Website).WithErrorMessage("Website must be a valid URL.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Website_Is_Valid()
        {
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567", "http://companya.com", DateTime.Now, DateTime.Now);
            var result = _validator.TestValidate(company);
            result.ShouldNotHaveValidationErrorFor(c => c.Website);
        }
    }
}
