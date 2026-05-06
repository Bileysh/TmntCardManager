using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class PlayMatchInputDto
{
    [Required]
    public int DeckId { get; set; }
    
    public int? TournamentId { get; set; }
}