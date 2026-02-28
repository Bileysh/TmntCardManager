
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Playerprofile
{
    public int Id { get; set; }
    [Display(Name = "Нікнейм")]
    public string Nickname { get; set; } = null!;
    [Display(Name = "Рейтинг")]
    public double? Winrate { get; set; }
    [Display(Name = "Аватарка")]
    public string? Avatarurl { get; set; }

    public virtual User? IdNavigation { get; set; } = null!;
}
