using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmntCardManager.Models;

public class UserCard
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public int CardId { get; set; }
    [ForeignKey("CardId")]
    public virtual Card Card { get; set; } = null!;

    public int Quantity { get; set; } = 1;
}