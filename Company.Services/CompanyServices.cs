using Company.Domain.ViewModels;
using Company.Persistence.Repository.Interfaces;
using Company.Services.Interfaces;

namespace Company.Services
{
    public class CompanyServices : ICompanyServices
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyServices(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IEnumerable<CompanyVM>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllAsync();
            return companies.Select(company => new CompanyVM
            {
                Id = company.Id,
                Name = company.Name,
                Exchange = company.Exchange,
                Ticker = company.Ticker,
                ISIN = company.ISIN,
                Website = company.Website
            });
        }

        public async Task<CompanyVM> GetCompanyByIdAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            return new CompanyVM
            {
                Id = company.Id,
                Name = company.Name,
                Exchange = company.Exchange,
                Ticker = company.Ticker,
                ISIN = company.ISIN,
                Website = company.Website
            };
        }

        public async Task<CompanyVM> GetCompanyByISINAsync(string ISIN)
        {
            var company = await _companyRepository.GetByISINAsync(ISIN);
            return new CompanyVM
            {
                Id = company.Id,
                Name = company.Name,
                Exchange = company.Exchange,
                Ticker = company.Ticker,
                ISIN = company.ISIN,
                Website = company.Website
            };
        }

        public async Task AddCompanyAsync(CompanyVM companyVM)
        {
            var company = new Company.Domain.Models.Company
            (
                0,
                companyVM.Name,
                companyVM.Exchange,
                companyVM.Ticker,
                companyVM.ISIN,
                companyVM.Website,
                DateTime.UtcNow,
                DateTime.UtcNow
            );
            await _companyRepository.AddAsync(company);
        }

        public async Task UpdateCompanyAsync(int id, CompanyVM companyVM)
        {
            if (id != companyVM.Id)
            {
                throw new Exception("Company ID cannot be updated.");
            }
            await _companyRepository.UpdateAsync(id, companyVM);
        }
    }
}
