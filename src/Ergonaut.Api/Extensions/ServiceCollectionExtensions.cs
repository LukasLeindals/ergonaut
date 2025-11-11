using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ergonaut.Api.Configuration;

namespace Ergonaut.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddErgonautPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ProjectsRead", policy => policy.RequireClaim("scope", "projects:read"));
            options.AddPolicy("ProjectsWrite", policy => policy.RequireClaim("scope", "projects:write"));
            options.AddPolicy("WorkItemsRead", policy => policy.RequireClaim("scope", "workitems:read"));
            options.AddPolicy("WorkItemsWrite", policy => policy.RequireClaim("scope", "workitems:write"));
        });

        return services;
    }

    public static IServiceCollection AddErgonautAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // temporary dev secrets; move to secure store later
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("Missing Jwt config");
        services.AddSingleton(jwtOptions);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidateLifetime = true
                };
            });


        return services;
    }

    public static IServiceCollection AddErgonautSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });

        return services;
    }



}
