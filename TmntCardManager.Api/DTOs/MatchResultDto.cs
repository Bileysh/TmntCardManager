namespace TmntCardManager.Api.DTOs;

public class MatchResultDto
{
    public  string MatchStatus { get; set; } = null!; 
    public string OpponentName { get; set; } = null!;
    public int EarnedCoins { get; set; }
    public int EarnedXp { get; set; }
    
    public List<string> BattleLog { get; set; } = new List<string>();
}