using Company.Persistence;
using Company.Persistence.Repository.Interfaces;
using Company.Persistence.Repository;
using CompanyAPI.APIs;
using Microsoft.EntityFrameworkCore;
using Company.Services.Interfaces;
using Company.Services;
using Company.Domain.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CompanyValidator>();

builder.Services.AddDbContext<CompanyContext>(
 o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyServices, CompanyServices>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations at startup only if there are pending migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CompanyContext>();
    var pendingMigrations = dbContext.Database.GetPendingMigrations();
    if (pendingMigrations.Any())
    {
        dbContext.Database.Migrate();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register the global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.RegisterCompanyEndpoints();

app.Run();

public partial class Program { }

