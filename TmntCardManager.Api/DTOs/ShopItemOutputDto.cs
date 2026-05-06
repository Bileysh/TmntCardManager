namespace TmntCardManager.Api.DTOs;

public class ShopItemOutputDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int PriceCoins { get; set; }
    public string? ImageUrl { get; set; }
}