using Mapster;
using MapsterMapper;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Mediatr.Commands.CreateTask;
using MinimalTaskControl.Infrastructure.Services;
using MinimalTaskControl.WebApi.DTOs.Tasks.Requests;

namespace MinimalTaskControl.WebApi.Extensions;

internal static class MapsterExtensions
{
    public static IServiceCollection AddMapsterMapping(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();

        ConfigureMappings(config);

        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IMappingService, MapsterMappingService>();

        return services;
    }

    private static void ConfigureMappings(TypeAdapterConfig config)
    {
        config.NewConfig<CreateTaskRequest, CreateTaskCommand>();
    }
}
