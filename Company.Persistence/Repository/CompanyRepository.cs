using Company.Domain.Exceptions.Database;
using Company.Domain.Exceptions;
using Company.Persistence.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Company.Domain.ViewModels;
using Company.Domain.Models;

namespace Company.Persistence.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CompanyContext _context;

        public CompanyRepository(CompanyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company.Domain.Models.Company>> GetAllAsync()
        {
            return await _context.Company.ToListAsync();
        }

        public async Task<Company.Domain.Models.Company> GetByIdAsync(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                throw new EntityNotFoundException($"Company with ID {id} not found.");
            }
            return company;
        }

        public async Task AddAsync(Company.Domain.Models.Company company)
        {
            var existingCompany = await _context.Company.FirstOrDefaultAsync(c => c.ISIN == company.ISIN);
            if (existingCompany != null)
            {
                throw new DuplicateEntityException($"Company with ISIN {company.ISIN} already exists.");
            }

            await _context.Company.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, CompanyVM companyVM)
        {
            var existingCompany = await _context.Company.FindAsync(id);
            if (existingCompany == null)
            {
                throw new EntityNotFoundException($"Company with ID {id} not found.");
            }

            var updatedCompany = new Domain.Models.Company
            (
                existingCompany.Id,
                companyVM.Name,
                companyVM.Exchange,
                companyVM.Ticker,
                companyVM.ISIN,
                companyVM.Website,
                existingCompany.CreatedOn,
                DateTime.UtcNow
            );

            _context.Entry(existingCompany).CurrentValues.SetValues(updatedCompany);

            await _context.SaveChangesAsync();
        }

        public async Task<Company.Domain.Models.Company> GetByISINAsync(string ISIN)
        {
            var company = await _context.Company.FirstOrDefaultAsync(o => o.ISIN == ISIN);
            if (company == null)
            {
                throw new EntityNotFoundException($"Company with ISIN {ISIN} not found.");
            }
            return company;
        }
    }
}
