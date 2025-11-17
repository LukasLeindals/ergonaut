
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ergonaut.App.Auth;
using Ergonaut.App.Extensions;
using Microsoft.Extensions.Options;


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

    public static IServiceCollection AddErgonautApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddErgonautAuth(configuration);
        services.AddSingleton<ITokenService, JwtTokenService>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(sp =>
        {
            var authSettings = sp.GetRequiredService<IOptions<AuthSettings>>().Value;

            return new ConfigureNamedOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authSettings.GetSigningKey(),
                    ValidateLifetime = true
                };
            });
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
