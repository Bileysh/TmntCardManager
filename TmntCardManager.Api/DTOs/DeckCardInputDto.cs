using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class DeckCardInputDto
{
    [Required(ErrorMessage = "ID колоди є обов'язковим")]
    public int DeckId { get; set; }

    [Required(ErrorMessage = "ID картки є обов'язковим")]
    public int CardId { get; set; }
}
