using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class ShopItemInputDto
{
    [Required(ErrorMessage = "Назва товару обов'язкова")]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Range(0, 100000)]
    public int PriceCoins { get; set; }
    
    public string? ImageUrl { get; set; }
}