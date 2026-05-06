namespace TmntCardManager.Api.DTOs;

public class DeckOutputDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public int OwnerId { get; set; }
    public string? OwnerName { get; set; } = string.Empty;
    public int TotalCards { get; set; }
}