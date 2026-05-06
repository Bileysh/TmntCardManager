using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class ShopItem
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public string Name { get; set; } = null!;
        
    public string? Description { get; set; }
        
    public int PriceCoins { get; set; }
        
    public string? ImageUrl { get; set; }
    
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}