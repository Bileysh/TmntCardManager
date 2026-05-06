using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class CreateTournamentDto
{
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int PrizeCoins { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
