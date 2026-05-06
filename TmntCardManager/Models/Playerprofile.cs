
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Playerprofile
{
    public int Id { get; set; }
    [Display(Name = "Нікнейм")]
    [Required(ErrorMessage = "Поле 'Нікнейм' є обов'язковим")]
    public string Nickname { get; set; } = null!;
    [Display(Name = "Рейтинг")]
    [Required(ErrorMessage = "Поле 'Рейтинг' є обов'язковим")]
    public double? Winrate { get; set; }
    [Display(Name = "Аватарка")]
    [Required(ErrorMessage = "Поле 'Аватарка' є обов'язковим")]
    public string? Avatarurl { get; set; }

    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int BalanceCoins { get; set; } = 0;
    
    public virtual User IdNavigation { get; set; } 
    public int? ActiveTournamentId { get; set; }
}
