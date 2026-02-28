
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Deckcard
{
    public int Id { get; set; }
    [Display(Name = "Колода")]
    public int Deckid { get; set; }
    [Display(Name = "Кількість")]
    public int Quantity { get; set; }
    [Display(Name = "Карта")]
    public int Cardid { get; set; }
    [Display(Name = "Карта")]
    public virtual Card? Card { get; set; } = null!;
    [Display(Name = "Колода")]
    public virtual Deck? Deck { get; set; } = null!;
}
