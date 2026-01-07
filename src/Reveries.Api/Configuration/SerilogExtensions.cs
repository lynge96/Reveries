using Serilog;

namespace Reveries.Api.Configuration;

public static class SerilogConfigurationExtensions
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}