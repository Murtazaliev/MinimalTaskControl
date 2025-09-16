using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MinimalTaskControl.Core.Enums;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Mediatr.Behaviors;

namespace MinimalTaskControl.Core.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddCoreFeatures(this IServiceCollection services)
    {
        services.AddMediatorFeatures();
        return services;
    }
    private static IServiceCollection AddMediatorFeatures(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddMediatR(cnf =>
        {
            cnf.RegisterServicesFromAssembly(typeof(TasksPriority).Assembly);

            cnf.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
