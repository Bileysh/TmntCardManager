using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class DeckInputDto
{
    [Required(ErrorMessage = "Назва колоди є обов'язковою")]
    public string Name { get; set; } = null!;
}