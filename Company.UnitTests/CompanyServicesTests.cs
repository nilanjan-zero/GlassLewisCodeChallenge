using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.Domain.ViewModels;
using Company.Persistence.Repository.Interfaces;
using Company.Services;
using Company.Services.Interfaces;
using Moq;
using Xunit;

namespace Company.UnitTests
{
    public class CompanyServicesTests
    {
        private readonly Mock<ICompanyRepository> _mockRepository;
        private readonly ICompanyServices _companyServices;

        public CompanyServicesTests()
        {
            _mockRepository = new Mock<ICompanyRepository>();
            _companyServices = new CompanyServices(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllCompaniesAsync_ReturnsAllCompanies()
        {
            // Arrange
            var companies = new List<Company.Domain.Models.Company>
            {
                new(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now),
                new(1, "Company B", "NYSE", "CB", "US987654321", "http://companyb.com", DateTime.Now, DateTime.Now),

            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(companies);

            // Act
            var result = await _companyServices.GetAllCompaniesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Company A", result.First().Name);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ReturnsCompany()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);
;
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _companyServices.GetCompanyByIdAsync(1);

            // Assert
            Assert.Equal("Company A", result.Name);
        }

        [Fact]
        public async Task GetCompanyByISINAsync_ReturnsCompany()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);
            _mockRepository.Setup(repo => repo.GetByISINAsync("US1234567890")).ReturnsAsync(company);

            // Act
            var result = await _companyServices.GetCompanyByISINAsync("US1234567890");

            // Assert
            Assert.Equal("Company A", result.Name);
        }

        [Fact]
        public async Task AddCompanyAsync_AddsCompany()
        {
            // Arrange
            var companyVM = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "A", ISIN = "US1234567890", Website = "http://companya.com" };

            // Act
            await _companyServices.AddCompanyAsync(companyVM);

            // Assert
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Company.Domain.Models.Company>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCompanyAsync_UpdatesCompany()
        {
            // Arrange
            var companyVM = new CompanyVM { Id = 1, Name = "Company A", Exchange = "NYSE", Ticker = "A", ISIN = "US1234567890", Website = "http://companya.com" };

            // Act
            await _companyServices.UpdateCompanyAsync(1, companyVM);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(1, companyVM), Times.Once);
        }

        [Fact]
        public async Task UpdateCompanyAsync_ThrowsException_WhenIdMismatch()
        {
            // Arrange
            var companyVM = new CompanyVM { Id = 2, Name = "Company A", Exchange = "NYSE", Ticker = "A", ISIN = "US1234567890", Website = "http://companya.com" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _companyServices.UpdateCompanyAsync(1, companyVM));
            Assert.Equal("Company ID cannot be updated.", exception.Message);
        }
    }
}
