using Asp.Versioning.ApiExplorer;
using MinimalTaskControl.Core.Extensions;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Infrastructure.Extensions;
using MinimalTaskControl.WebApi;
using MinimalTaskControl.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebApiFeatures(builder.Configuration)
    .AddCoreFeatures()
    .AddInfrastructureFeatures(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"MinimalTaskControl API {description.GroupName}");
        }
    });

    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
