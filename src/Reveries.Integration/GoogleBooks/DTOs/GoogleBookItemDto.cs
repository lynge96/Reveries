namespace Reveries.Integration.GoogleBooks.DTOs;

public sealed record GoogleBookItemDto
{
    public string? Id { get; init; }
    public GoogleVolumeInfoDto? VolumeInfo { get; init; }
}