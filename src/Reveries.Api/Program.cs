using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Reveries.Api.Configuration.Cors;
using Reveries.Api.Configuration.ExceptionHandling;
using Reveries.Api.Configuration.HealthCheck;
using Reveries.Api.Configuration.OpenApi;
using Reveries.Api.Endpoints;
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
    .AddExceptionHandling(builder.Environment)
    .AddOpenApiDocument(builder.Configuration);

var app = builder.Build();

DatabaseMigrator.Run(
    app.Configuration.GetConnectionString(ConnectionStringKeys.ReveriesDb)!,
    app.Services.GetRequiredService<ILoggerFactory>());

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiDocumentation(app.Configuration);
}

app.MapStandardHealthChecks("/healthz");

app.UseCors(app.Environment.IsDevelopment() ? "Development" : "AllowFrontend");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapBookEndpoints();

app.Run();