namespace TmntCardManager.Api.DTOs;

public class CardClassOutputDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}