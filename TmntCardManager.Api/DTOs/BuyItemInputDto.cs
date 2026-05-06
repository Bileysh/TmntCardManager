using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class BuyItemInputDto
{
    [Required]
    public int ShopItemId { get; set; }
}