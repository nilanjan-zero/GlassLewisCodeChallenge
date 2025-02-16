using System.Net.Http.Json;
using System.Threading.Tasks;
using Company.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace Company.IntegrationTests
{
    public class CompanyEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CompanyEndpointsTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetCompanyById_ReturnsCompany()
        {
            var response = await _client.GetAsync("/company/id:1");
            response.EnsureSuccessStatusCode();

            var company = await response.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(company);
            Assert.Equal(1, company.Id);
        }

        [Fact]
        public async Task GetCompanyByISIN_ReturnsCompany()
        {
            var response = await _client.GetAsync("/company/isin:US1234567890");
            response.EnsureSuccessStatusCode();

            var company = await response.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(company);
            Assert.Equal("US1234567890", company.ISIN);
        }

        [Fact]
        public async Task GetAllCompanies_ReturnsCompanies()
        {
            var response = await _client.GetAsync("/all-companies");
            response.EnsureSuccessStatusCode();

            var companies = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyVM>>();
            Assert.NotNull(companies);
            Assert.NotEmpty(companies);
        }

        [Fact]
        public async Task AddCompany_ReturnsOk()
        {
            var newCompany = new CompanyVM
            {
                Name = "New Company",
                Exchange = "NYSE",
                Ticker = "NEW",
                ISIN = "US0987654321",
                Website = "http://newcompany.com"
            };

            var response = await _client.PostAsJsonAsync("/add-company", newCompany);
            response.EnsureSuccessStatusCode();

            var returnedCompany = await response.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(returnedCompany);
            Assert.Equal(newCompany.Id, returnedCompany.Id);
        }

        [Fact]
        public async Task UpdateCompany_ReturnsOk()
        {
            var updatedCompany = new CompanyVM
            {
                Id = 1,
                Name = "Updated Company",
                Exchange = "NASDAQ",
                Ticker = "UPD",
                ISIN = "US1234567890",
                Website = "http://updatedcompany.com"
            };

            var response = await _client.PatchAsJsonAsync("/update-company/id:1", updatedCompany);
            response.EnsureSuccessStatusCode();

            var returnedCompany = await response.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(returnedCompany);
            Assert.Equal(updatedCompany.Name, returnedCompany.Name);
        }
    }
}