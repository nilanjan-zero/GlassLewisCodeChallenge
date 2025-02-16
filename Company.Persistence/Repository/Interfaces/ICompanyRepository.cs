using Company.Domain.ViewModels;

namespace Company.Persistence.Repository.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company.Domain.Models.Company>> GetAllAsync();
        Task<Company.Domain.Models.Company> GetByIdAsync(int id);
        Task<Company.Domain.Models.Company> GetByISINAsync(string ISIN);
        Task AddAsync(Company.Domain.Models.Company company);
        Task UpdateAsync(int id, CompanyVM company);
    }
}