namespace Reveries.Application.DTOs.GoogleBooksDtos;

public class GoogleBookItemDto
{
    public string Id { get; set; } = string.Empty;
    public GoogleVolumeInfoDto VolumeInfo { get; set; } = new();
}