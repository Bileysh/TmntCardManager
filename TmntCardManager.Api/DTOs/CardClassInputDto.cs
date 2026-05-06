using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class CardClassInputDto
{
    [Required(ErrorMessage = "Назва фракції є обов'язковою")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}