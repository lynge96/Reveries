using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Reveries.Api.Configuration.Cors;
using Reveries.Api.Configuration.ExceptionHandling;
using Reveries.Api.Configuration.HealthCheck;
using Reveries.Api.Configuration.OpenApi;
using Reveries.Api.Configuration.RequestTimeouts;
using Reveries.Api.Endpoints;
using Reveries.Application;
using Reveries.Api.Configuration.Logging;
using Reveries.Application.Common.Caching;
using Reveries.Integration;
using Reveries.Persistence.Configuration;
using Reveries.Persistence.Migrations;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});
builder.Services.AddApplicationHealthChecks();
builder.Services.AddValidation();
builder.Services.AddRequestTimeoutPolicies();

builder.Services
    .AddApplication()
    .AddPostgres(builder.Configuration)
    .AddIntegration(builder.Configuration)
    .AddBookSearchCaching(builder.Configuration)
    .AddCorsPolicies()
    .AddExceptionHandling(builder.Environment)
    .AddOpenApiDocument(builder.Configuration)
    .AddDatabaseMigrations();

var app = builder.Build();

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
app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseRequestTimeouts();

app.MapBookEndpoints();

app.Run();