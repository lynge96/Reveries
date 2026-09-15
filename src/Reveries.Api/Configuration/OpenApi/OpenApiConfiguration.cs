namespace Reveries.Api.Configuration.OpenApi;

public sealed class OpenApiConfiguration
{
    public string Title { get; set; } = "Reveries API";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = string.Empty;
    public List<ServerInfo>? Servers { get; set; }
}

public sealed class ServerInfo
{
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}