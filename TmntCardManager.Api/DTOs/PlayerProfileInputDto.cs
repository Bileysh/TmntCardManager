using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class PlayerProfileInputDto
{
    [Required(ErrorMessage = "Нікнейм не може бути порожнім")]
    public string Nickname { get; set; } = null!;
    
    public string? Avatarurl { get; set; }
}