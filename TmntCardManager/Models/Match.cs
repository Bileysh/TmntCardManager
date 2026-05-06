using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmntCardManager.Models;

public class Match
{
    [Key]
    public int Id { get; set; }
    public int Player1Id { get; set; }
    [ForeignKey("Player1Id")]
    public virtual User Player1 { get; set; } = null!;
    public int Player2Id { get; set; }
    [ForeignKey("Player2Id")]
    public virtual User Player2 { get; set; } = null!;
    public int? WinnerId { get; set; }
    [ForeignKey("WinnerId")]
    public virtual User? Winner { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    
    public int? TournamentId { get; set; }
    [ForeignKey("TournamentId")]
    public virtual Tournament? Tournament { get; set; }
}