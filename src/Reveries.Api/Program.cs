using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Reveries.Api.Configuration;
using Reveries.Api.Middleware;
using Reveries.Application;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services
    .AddApplicationServices()
    .AddPostgresql(builder.Configuration)
    .AddRedisCache(builder.Configuration)
    .AddIsbndbServices(builder.Configuration)
    .AddGoogleBooksServices(builder.Configuration)
    .AddCorsPolicies()
    .AddSwaggerDocumentation()
    .AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}
        
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(app.Environment.IsDevelopment() ? "Development" : "AllowFrontend");

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();