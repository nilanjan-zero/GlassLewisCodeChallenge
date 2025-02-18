using System.Net.Http.Json;
using System.Threading.Tasks;
using Company.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Company.IntegrationTests
{
    public class CompanyEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CompanyEndpointsTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetCompanyById_ReturnsCompany()
        {
            var newCompany = new CompanyVM
            {
                Name = "New Company",
                Exchange = "NYSE",
                Ticker = "NEW",
                ISIN = $"US{DateTime.Now.Ticks}",
                Website = "http://newcompany.com"
            };

            var addCompanyResponse = await _client.PostAsJsonAsync("/add-company", newCompany);
            addCompanyResponse.EnsureSuccessStatusCode();
            var addedCompany = await addCompanyResponse.Content.ReadFromJsonAsync<CompanyVM>();

            var getAllCompaniesResponse = await _client.GetAsync("/all-companies");
            getAllCompaniesResponse.EnsureSuccessStatusCode();

            var companies = await getAllCompaniesResponse.Content.ReadFromJsonAsync<IEnumerable<CompanyVM>>();
            Assert.NotNull(companies);

            var getCompanyByIdResponse = await _client.GetAsync($"/company/id:{companies.First().Id}");
            getCompanyByIdResponse.EnsureSuccessStatusCode();

            var company = await getCompanyByIdResponse.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(company);
            Assert.Equal(companies.First().Id, company.Id);
        }

        [Fact]
        public async Task GetCompanyByISIN_ReturnsCompany()
        {
            var newCompany = new CompanyVM
            {
                Name = "New Company",
                Exchange = "NYSE",
                Ticker = "NEW",
                ISIN = $"US{DateTime.Now.Ticks}",
                Website = "http://newcompany.com"
            };

            var addCompanyResponse = await _client.PostAsJsonAsync("/add-company", newCompany);
            addCompanyResponse.EnsureSuccessStatusCode();
            var addedCompany = await addCompanyResponse.Content.ReadFromJsonAsync<CompanyVM>();

            var getCompanyByISINResponse = await _client.GetAsync($"/company/isin:{addedCompany.ISIN}");
            getCompanyByISINResponse.EnsureSuccessStatusCode();

            var company = await getCompanyByISINResponse.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(company);
            Assert.Equal(newCompany.ISIN, company.ISIN);
        }

        [Fact]
        public async Task GetAllCompanies_ReturnsCompanies()
        {
            var newCompany = new CompanyVM
            {
                Name = "New Company",
                Exchange = "NYSE",
                Ticker = "NEW",
                ISIN = $"US{DateTime.Now.Ticks}",
                Website = "http://newcompany.com"
            };

            var addCompanyResponse = await _client.PostAsJsonAsync("/add-company", newCompany);
            addCompanyResponse.EnsureSuccessStatusCode();

            var getAllCompaniesResponse = await _client.GetAsync("/all-companies");
            getAllCompaniesResponse.EnsureSuccessStatusCode();

            var companies = await getAllCompaniesResponse.Content.ReadFromJsonAsync<IEnumerable<CompanyVM>>();
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
                ISIN = $"US{DateTime.Now.Ticks}",
                Website = "http://newcompany.com"
            };

            var addCompanyResponse = await _client.PostAsJsonAsync("/add-company", newCompany);
            addCompanyResponse.EnsureSuccessStatusCode();

            var returnedCompany = await addCompanyResponse.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(returnedCompany);
            Assert.Equal(newCompany.ISIN, returnedCompany.ISIN);
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
                ISIN = "US1234567891",
                Website = "http://updatedcompany.com"
            };

            var updateCompanyResponse = await _client.PatchAsJsonAsync("/update-company/id:1", updatedCompany);
            updateCompanyResponse.EnsureSuccessStatusCode();

            var returnedCompany = await updateCompanyResponse.Content.ReadFromJsonAsync<CompanyVM>();
            Assert.NotNull(returnedCompany);
            Assert.Equal(updatedCompany.Name, returnedCompany.Name);
        }

        [Fact]
        public async Task GetCompanyById_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/company/id:9999");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCompanyByISIN_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/company/isin:12INVALIDISIN");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddCompany_ReturnsBadRequest_WhenInvalidData()
        {
            var invalidCompany = new CompanyVM
            {
                Name = "",
                Exchange = "NYSE",
                Ticker = "NEW",
                ISIN = "12INVALIDISIN", // Invalid ISIN
                Website = "http://newcompany.com"
            };

            var response = await _client.PostAsJsonAsync("/add-company", invalidCompany);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCompany_ReturnsNotFound()
        {
            var updatedCompany = new CompanyVM
            {
                Id = 9999, // Non-existent ID
                Name = "Updated Company",
                Exchange = "NASDAQ",
                Ticker = "UPD",
                ISIN = "US1234567891",
                Website = "http://updatedcompany.com"
            };

            var response = await _client.PatchAsJsonAsync("/update-company/id:9999", updatedCompany);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCompany_ReturnsBadRequest_WhenInvalidData()
        {
            var invalidCompany = new CompanyVM
            {
                Id = 1,
                Name = "", // Invalid name
                Exchange = "NASDAQ",
                Ticker = "UPD",
                ISIN = "INVALIDISIN", // Invalid ISIN
                Website = "http://updatedcompany.com"
            };

            var response = await _client.PatchAsJsonAsync("/update-company/id:1", invalidCompany);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
