using Ergonaut.App.Extensions;
using Ergonaut.Api.Extensions;
using Ergonaut.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddErgonautSwagger();

builder.Services.AddErgonautPolicies();
builder.Services.AddErgonautAuthentication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.ConfigureRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

