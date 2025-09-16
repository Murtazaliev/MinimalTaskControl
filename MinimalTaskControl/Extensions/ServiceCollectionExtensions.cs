using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalTaskControl.Core.Models;
using MinimalTaskControl.WebApi.Filters;
using System.Text;
using System.Text.Json.Serialization;

namespace MinimalTaskControl.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiFeatures(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddApiVersioningAndExplorer();
            services.AddSwaggerDocumentation();
            services.AddJwtAuthentication(configuration);

            services.AddMapsterMapping();

            return services;
        }

        private static IServiceCollection AddApiVersioningAndExplorer(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = $"MinimalTaskControl API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token in format: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.OperationFilter<SwaggerDefaultValues>();
            });

            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
