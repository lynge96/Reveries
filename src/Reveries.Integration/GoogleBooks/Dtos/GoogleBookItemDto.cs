namespace Reveries.Integration.GoogleBooks.Dtos;

public sealed record GoogleBookItemDto
{
    public string? Id { get; init; }
    public GoogleVolumeInfoDto? VolumeInfo { get; init; }
}