using DotNetEnv;
using Reveries.Api.Configuration;
using Reveries.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (!File.Exists(".env"))
    throw new FileNotFoundException("Missing .env file in project root");
Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddReveriesServices()
    .AddCorsPolicies()
    .AddSwaggerDocumentation()
    .AddControllers();

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