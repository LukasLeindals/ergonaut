using System.Text;
using Ergonaut.App.Features.Projects;
using Ergonaut.App.Features.WorkItems;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ergonaut.Api.Configuration;
using Ergonaut.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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


// temporary dev secrets; move to secure store later
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Missing Jwt config");
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProjectsRead", policy => policy.RequireClaim("scope", "projects:read"));
    options.AddPolicy("ProjectsWrite", policy => policy.RequireClaim("scope", "projects:write"));
    options.AddPolicy("WorkItemsRead", policy => policy.RequireClaim("scope", "workitems:read"));
    options.AddPolicy("WorkItemsWrite", policy => policy.RequireClaim("scope", "workitems:write"));
});


builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
// reuse the shared application services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectFactory, LocalProjectFactory>();
builder.Services.AddScoped<IProjectScopedWorkItemService, LocalProjectScopedWorkItemService>();
builder.Services.AddScoped<IWorkItemService>(sp => sp.GetRequiredService<IProjectScopedWorkItemService>());


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

