namespace Reveries.Api.Configuration.Swagger;

public class SwaggerConfiguration
{
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LicenseInfo? License { get; set; }
    public bool EnableSecurity { get; set; }
    public bool EnableXmlComments { get; set; }
    public List<ServerInfo>? Servers { get; set; }
}

public class LicenseInfo
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class ServerInfo
{
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}