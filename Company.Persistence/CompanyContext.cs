using Microsoft.EntityFrameworkCore;
using Company.Domain.Models;

namespace Company.Persistence
{
    public class CompanyContext(DbContextOptions<CompanyContext> options) : DbContext(options)
    {
        public virtual DbSet<Company.Domain.Models.Company> Company { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
