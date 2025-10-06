using Reveries.Application.Services.Configuration;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddIsbndbServices()
    .AddGoogleBooksServices()
    .AddRedisCacheServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Development");

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();