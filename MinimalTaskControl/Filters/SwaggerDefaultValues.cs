using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace MinimalTaskControl.WebApi.Filters
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Parameters ??= new List<OpenApiParameter>();
            var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "version");
            if (versionParameter != null)
            {
                operation.Parameters.Remove(versionParameter);
            }

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions
                    .First(p => p.Name == parameter.Name);

                parameter.Description ??= description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(
                        JsonSerializer.Serialize(description.DefaultValue));
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
