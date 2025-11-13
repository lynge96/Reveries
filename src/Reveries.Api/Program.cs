using DotNetEnv;
using FluentValidation;
using Reveries.Api.Configuration;
using Reveries.Api.Interfaces;
using Reveries.Api.Middleware;
using Reveries.Api.Services;
using Reveries.Application.Configuration;
using Reveries.Infrastructure.Configuration;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();

builder.Services
    .AddAppConfiguration(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructure()
    .AddIsbndbServices()
    .AddGoogleBooksServices()
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