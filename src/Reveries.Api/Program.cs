using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Reveries.Api.Configuration.Cors;
using Reveries.Api.Configuration.HealthCheck;
using Reveries.Api.Configuration.Swagger;
using Reveries.Api.Middleware;
using Reveries.Application;
using Reveries.Infrastructure;
using Reveries.Infrastructure.Logging;
using Reveries.Integration;
using Reveries.Persistence;
using Reveries.Persistence.Migrations;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});
builder.Services.AddApplicationHealthChecks();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddIntegration(builder.Configuration)
    .AddCorsPolicies()
    .AddSwagger(builder.Configuration)
    .AddControllers();

var app = builder.Build();

DatabaseMigrator.Run(
    app.Configuration.GetConnectionString(ConnectionStringKeys.ReveriesDb)!,
    app.Services.GetRequiredService<ILoggerFactory>());

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(app.Configuration);
}

app.MapStandardHealthChecks("/healthz");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(app.Environment.IsDevelopment() ? "Development" : "AllowFrontend");
app.UseSerilogRequestLogging();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
