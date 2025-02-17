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

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbConnection));

                if (dbConnectionDescriptor != null)
                {
                    services.Remove(dbConnectionDescriptor);
                }

                // Create SQL Server connection
                services.AddSingleton<DbConnection>(container =>
                {
                    var masterConnectionString = "Server=localhost,1433; Initial Catalog=master; User Id=SA; Password=Easypass123; TrustServerCertificate=True;MultipleActiveResultSets=true";
                    using (var masterConnection = new SqlConnection(masterConnectionString))
                    {
                        masterConnection.Open();
                        CreateDatabase(masterConnection);
                    }

                    var testDbConnectionString = "Server=localhost,1433; Initial Catalog=CompanyTestDb; User Id=SA; Password=Easypass123; TrustServerCertificate=True;MultipleActiveResultSets=true";
                    _connection = new SqlConnection(testDbConnectionString);
                    _connection.Open();
                    return _connection;
                });

                services.AddDbContext<CompanyContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlServer(connection);
                });

                // Ensure the database is created and apply migrations
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CompanyContext>();
                    dbContext.Database.Migrate();
                    SeedTestData(dbContext);
                }
            });

            builder.UseEnvironment("Development");
        }

        private void SeedTestData(CompanyContext context)
        {
            // Seed test data
            context.Company.FromSqlRaw("INSERT INTO [dbo].[Company] ([Name]," +
                "[Exchange],[Ticker],[ISIN],[Website],[CreatedOn],[UpdatedOn]) " +
                "VALUES ('Company A' ,'NYSE' ,'CA','US1234567890','abc.com',GETDATE(),GETDATE())");
            context.SaveChanges();
        }

        private void CreateDatabase(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CompanyTestDb') " +
                    "BEGIN " +
                    "CREATE DATABASE [CompanyTestDb]; " +
                    "END";
                command.ExecuteNonQuery();
            }
        }

        public new void Dispose()
        {
            // Drop the database after tests
            if (_connection != null)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = "DROP DATABASE IF EXISTS [CompanyTestDb]";
                    command.ExecuteNonQuery();
                }

                _connection.Close();
                _connection.Dispose();
            }

            base.Dispose();
        }
    }
}
