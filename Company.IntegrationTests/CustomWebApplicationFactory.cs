using Company.Domain.ViewModels;
using Company.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Linq;

namespace Company.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IDisposable where TProgram : class
    {
        private DbConnection _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CompanyContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                // Create SQL Server connection
                services.AddSingleton<DbConnection>(container =>
                {//    "SqlServer": "Server=sqldatabase; Database=CompanyDb; User Id=SA; Password=Easypass123; TrustServerCertificate=True;MultipleActiveResultSets=true"

                    //var connectionString = "Data Source=.;Initial Catalog=CompanyTestDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                    var connectionString = "Server=sqldatabase; Database=CompanyTestDb; User Id=SA; Password=Easypass123; TrustServerCertificate=True;MultipleActiveResultSets=true";
                    _connection = new SqlConnection(connectionString);
                    _connection.Open();

                    return _connection;
                });

                services.AddDbContext<CompanyContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlServer(connection);
                });

                // Ensure the database is created and seed test data
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<CompanyContext>();
                    //db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    SeedTestData(db);
                }
            });

            builder.UseEnvironment("Development");
        }

        private void SeedTestData(CompanyContext context)
        {
            // Seed test data
            context.Company.FromSqlRaw("INSERT INTO [dbo].[Company] ([Name]," +
                "[Exchange],[Ticker],[ISIN],[Website],[CreatedOn],[UpdatedOn]) " +
                "VALUES ('Company A' ,'NYSE' ,'CA','US123456','abc.com',GETDATE(),GETDATE())");
            context.SaveChanges();
        }

        public new void Dispose()
        {
            // Drop the database after tests
            if (_connection != null)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = "DROP DATABASE [CompanyTestDb]";
                    command.ExecuteNonQuery();
                }

                _connection.Close();
                _connection.Dispose();
            }

            base.Dispose();
        }
    }
}
