namespace TmntCardManager.Api.DTOs;

public class BuyResultDto
{
    public string Message { get; set; } = null!;
    public int NewBalance { get; set; }
    public List<string> ReceivedCards { get; set; } = new List<string>();
}