using FluentValidation;
using Reveries.Api.Configuration;
using Reveries.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();

builder.Services.AddAppConfiguration(builder.Configuration, builder.Environment);

builder.Services
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