using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Infrastructure.Database;
using MinimalTaskControl.Infrastructure.Database.Repositories;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Infrastructure.Database.Data;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Infrastructure.Services;
using MinimalTaskControl.Infrastructure.Specifications;

namespace MinimalTaskControl.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureFeatures(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
    {
        services.AddDatabase(configuration, environment);
        services.AddImplementations();
        return services;
    }

    private static IServiceCollection AddImplementations(this IServiceCollection services)
    {
        services.AddScoped<ISpecificationFactory, SpecificationFactory>();
        services.AddScoped<IDataSeeder, InMemoryDataSeeder>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddTransient<ITaskInfoRepository, TaskInfoRepository>();
        services.AddTransient<IRepository<TaskInfo>, Repository<TaskInfo>>();
        return services;
    }

    private static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
    {
        // Проверяем, какой провайдер использовать
        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");

        if (useInMemory && environment.IsDevelopment())
        {
            services.AddInMemoryDatabase();
        }
        else
        {
            services.AddPostgreSqlDatabase(configuration, environment);
        }

        return services;
    }


    private static IServiceCollection AddInMemoryDatabase(
            this IServiceCollection services)
    {
        services.AddDbContext<MinimalTaskControlDbContext>(options =>
        {
            options.UseInMemoryDatabase("MinimalTaskControlDb");

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();

            options.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
        });

        return services;
    }

    private static IServiceCollection AddPostgreSqlDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContext<MinimalTaskControlDbContext>((serviceProvider, options) =>
        {
            var defaultConnection = "DefaultConnection";
            var connectionString = configuration.GetConnectionString(defaultConnection) ??
                throw new ArgumentNullException(defaultConnection, $"{defaultConnection} строка подключения должна быть предоставлена");

            options.UseNpgsql(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(MinimalTaskControlDbContext).Assembly.FullName);
                sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, MinimalTaskControlDbContext.DefaultSchema);
            });

            options.EnableSensitiveDataLogging(!environment.IsProduction());
            options.UseSnakeCaseNamingConvention();
        });

        return services;
    }
}
