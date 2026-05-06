namespace TmntCardManager.Api.DTOs;

public class CardOutputDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public int? Strength { get; set; }
    public int? Agility { get; set; }
    public int? Skill { get; set; }
    public int? Wit { get; set; }
    public int? ClassId { get; set; }
    
    public string ClassName { get; set; } = string.Empty; 
}