using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class CardInputDto
{
    [Required(ErrorMessage = "Назва картки є обов'язковою")]
    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    [Range(1000, 9999, ErrorMessage = "Сила має бути від 1000 до 9999")]
    public int Strength { get; set; }

    [Range(1000, 9999, ErrorMessage = "Спритність має бути від 1000 до 9999")]
    public int Agility { get; set; }

    [Range(1000, 9999, ErrorMessage = "Навичка має бути від 1000 до 9999")]
    public int Skill { get; set; }

    [Range(1000, 9999, ErrorMessage = "Інтелект має бути від 1000 до 9999")]
    public int Wit { get; set; }

    [Required(ErrorMessage = "ID класу (фракції) є обов'язковим")]
    public int ClassId { get; set; }
}