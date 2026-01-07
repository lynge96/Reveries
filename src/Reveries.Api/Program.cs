using DotNetEnv;
using FluentValidation;
using Reveries.Api.Configuration;
using Reveries.Api.Interfaces;
using Reveries.Api.Middleware;
using Reveries.Api.Services;
using Reveries.Application.Configuration;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.GoogleBooks.Configuration;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();

builder.Services
    .AddApplicationServices()
    .AddPostgresql(builder.Configuration)
    .AddRedisCache(builder.Configuration)
    .AddIsbndbServices(builder.Configuration)
    .AddGoogleBooksServices(builder.Configuration)
    .AddScoped<IBookService, BookService>()
    .AddCorsPolicies()
    .AddSwaggerDocumentation()
    .AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}
        
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("Development");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();