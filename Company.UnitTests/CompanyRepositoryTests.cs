using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.Domain.Exceptions.Database;
using Company.Domain.Models;
using Company.Domain.ViewModels;
using Company.Persistence;
using Company.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Company.UnitTests
{
    public class CompanyRepositoryTests : IDisposable
    {
        private readonly CompanyContext _context;
        private readonly CompanyRepository _companyRepository;

        public CompanyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CompanyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CompanyContext(options);
            _companyRepository = new CompanyRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCompanies()
        {
            // Arrange
            _context.Company.AddRange(
                new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now),
                new Domain.Models.Company(2, "Company B", "NASDAQ", "CA", "US0987654321", "http://companyb.com", DateTime.Now, DateTime.Now)
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _companyRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Company A", result.First().Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCompany()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);
            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            // Act
            var result = await _companyRepository.GetByIdAsync(1);

            // Assert
            Assert.Equal("Company A", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsEntityNotFoundException_WhenCompanyNotFound()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _companyRepository.GetByIdAsync(99));
            Assert.Equal("Company with ID 99 not found.", exception.Message);
        }

        [Fact]
        public async Task AddAsync_AddsCompany()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);

            // Act
            await _companyRepository.AddAsync(company);

            // Assert
            var result = await _context.Company.FindAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Company A", result.Name);
        }

        [Fact]
        public async Task AddAsync_ThrowsDuplicateEntityException_WhenCompanyWithSameISINExists()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);
            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            // Act & Assert
            var duplicateCompany = new Domain.Models.Company(2, "Company B", "NASDAQ", "CA", "US1234567890", "http://companyb.com", DateTime.Now, DateTime.Now);
            var exception = await Assert.ThrowsAsync<DuplicateEntityException>(() => _companyRepository.AddAsync(duplicateCompany));
            Assert.Equal("Company with ISIN US1234567890 already exists.", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesCompany()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);
            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            var companyVM = new CompanyVM { Id = 1, Name = "Updated Company A", Exchange = "NYSE", Ticker = "A", ISIN = "US1234567890", Website = "http://updatedcompanya.com" };

            // Act
            await _companyRepository.UpdateAsync(1, companyVM);

            // Assert
            var result = await _context.Company.FindAsync(1);
            Assert.Equal("Updated Company A", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsEntityNotFoundException_WhenCompanyNotFound()
        {
            // Arrange
            var companyVM = new CompanyVM { Id = 1, Name = "Updated Company A", Exchange = "NYSE", Ticker = "A", ISIN = "US1234567890", Website = "http://updatedcompanya.com" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _companyRepository.UpdateAsync(99, companyVM));
            Assert.Equal("Company with ID 99 not found.", exception.Message);
        }

        [Fact]
        public async Task GetByISINAsync_ReturnsCompany()
        {
            // Arrange
            var company = new Domain.Models.Company(1, "Company A", "NYSE", "CA", "US1234567890", "http://companya.com", DateTime.Now, DateTime.Now);
            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            // Act
            var result = await _companyRepository.GetByISINAsync("US1234567890");

            // Assert
            Assert.Equal("Company A", result.Name);
        }

        [Fact]
        public async Task GetByISINAsync_ThrowsEntityNotFoundException_WhenCompanyNotFound()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _companyRepository.GetByISINAsync("US9999999999"));
            Assert.Equal("Company with ISIN US9999999999 not found.", exception.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
