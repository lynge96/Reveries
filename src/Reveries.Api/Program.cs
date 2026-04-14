using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Reveries.Api.Configuration.Cors;
using Reveries.Api.Configuration.Swagger;
using Reveries.Api.Middleware;
using Reveries.Application;
using Reveries.Infrastructure.Common;
using Reveries.Infrastructure.Common.Logging;
using Reveries.Integration;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddIntegrations(builder.Configuration)
    .AddCorsPolicies()
    .AddSwagger()
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
app.UseSerilogRequestLogging();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();