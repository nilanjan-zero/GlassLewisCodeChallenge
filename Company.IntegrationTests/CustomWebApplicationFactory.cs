using Company.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0#test-app-prerequisites
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
                    // TODO: put connection string in appsettings.json
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
                using var scope = sp.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<CompanyContext>();
                dbContext.Database.Migrate();
                //SeedTestData(dbContext);

            });

            builder.UseEnvironment("Development");
        }

        // TODO: fix this method
        private void SeedTestData(CompanyContext context)
        {
            // Enable identity insert and seed test data
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Company] ON");
            context.Company.FromSqlRaw("INSERT INTO [dbo].[Company] ([ID],[Name]," +
                "[Exchange],[Ticker],[ISIN],[Website],[CreatedOn],[UpdatedOn]) " +
                "VALUES (1, 'Company A', 'NYSE', 'CA', 'US1234567890', 'companya.com', GETDATE(), GETDATE())");
            context.Company.FromSqlRaw("INSERT INTO [dbo].[Company] ([ID],[Name]," +
                "[Exchange],[Ticker],[ISIN],[Website],[CreatedOn],[UpdatedOn]) " +
                "VALUES (2, 'Company B', 'NYSE', 'CB', 'US0987654321', 'companyb.com', GETDATE(), GETDATE())");
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Company] OFF");
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

        //TODO: fix this method DROP DATABASE.
        public new void Dispose()
        {
            if (_connection != null)
            {
                using (var command = _connection.CreateCommand())
                {
                    // Drop the database
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
