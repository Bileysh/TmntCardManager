
namespace TmntCardManager.Models;

public class Playerprofile
{
    public int Id { get; set; }

    public string Nickname { get; set; } = null!;

    public double? Winrate { get; set; }

    public string? Avatarurl { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
