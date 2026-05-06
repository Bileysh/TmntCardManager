using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmntCardManager.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    public int ShopItemId { get; set; }
    [ForeignKey("ShopItemId")]
    public virtual ShopItem ShopItem { get; set; } = null!;
    public int SpentCoins { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}