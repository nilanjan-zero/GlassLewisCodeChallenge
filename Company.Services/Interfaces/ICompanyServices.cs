using Company.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Interfaces
{
    public interface ICompanyServices
    {
        Task<IEnumerable<CompanyVM>> GetAllCompaniesAsync();
        Task<CompanyVM> GetCompanyByIdAsync(int id);
        Task<CompanyVM> GetCompanyByISINAsync(string ISIN);
        Task AddCompanyAsync(CompanyVM company);
        Task UpdateCompanyAsync(int id, CompanyVM company);
    }
}
