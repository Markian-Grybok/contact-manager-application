using AutoMapper;
using ContactManager.BLL.Interfaces;
using ContactManager.BLL.Mapping;
using ContactManager.BLL.Services;
using ContactManager.BLL.Validators;
using ContactManager.DAL;
using ContactManager.DAL.Repositories.Interfaces;
using ContactManager.DAL.Repositories.Realizations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebAPI.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<ContactCreateViewModelValidator>();

        services.AddAutoMapper(config =>
        {
            config.AddProfile<ContactProfile>();
        });

        services.AddDbContext<ContactManagerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DataBaseConnection")));

        services.AddScoped<IContactRepository, ContactRepository>();
        
        services.AddScoped<ICsvService, CsvService>();

        return services;
    }
}
