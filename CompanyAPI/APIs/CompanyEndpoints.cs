using Company.Domain.ViewModels;
using Company.Services.Interfaces;

namespace CompanyAPI.APIs
{
    public static class CompanyEndpoints
    {
        public static void RegisterCompanyEndpoints(this WebApplication app)
        {
            app.MapGet("/company/id:{id}", async (int id, ICompanyServices companyServices) =>
            {
                var company = await companyServices.GetCompanyByIdAsync(id);

                return company;
            });

            app.MapGet("/company/isin:{ISIN}", async (string ISIN, ICompanyServices companyServices) =>
            {
                var company = await companyServices.GetCompanyByISINAsync(ISIN);
                return company;
            });

            app.MapGet("/all-companies", async (ICompanyServices companyServices) =>
            {
                var companies = await companyServices.GetAllCompaniesAsync();
                return companies;
            });

            app.MapPost("/add-company", async (CompanyVM companyVM, ICompanyServices companyServices) =>
            {
                await companyServices.AddCompanyAsync(companyVM);
                return Results.Ok(companyVM);
            });

            app.MapPatch("/update-company/id:{id}", async (int id, CompanyVM companyVM, ICompanyServices companyServices) =>
            {
                await companyServices.UpdateCompanyAsync(id, companyVM);
                return Results.Ok(companyVM);
            });
        }
    }

}