using DotNetEnv;
using FluentValidation;
using Reveries.Api.Configuration;
using Reveries.Api.Middleware;

if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();

builder.Services
    .AddAppConfiguration(builder.Configuration, builder.Environment)
    .AddReveriesServices()
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