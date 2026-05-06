namespace TmntCardManager.Api.DTOs;

public class PlayerProfileOutputDto
{
    public int Id { get; set; }
    public string Nickname { get; set; } = null!;
    public double? Winrate { get; set; }
    public string? Avatarurl { get; set; }
    
    public int Level { get; set; } 
    public int Experience { get; set; } 
    public int BalanceCoins { get; set; }
    
}