
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Deck
{
    public int Id { get; set; }
    [Display(Name = "Назва колоди")]
    [Required(ErrorMessage = "Поле 'Назва колоди' є обов'язковим")]
    public string Name { get; set; } = null!;
    [Display(Name = "Дата створення")]
    public DateTime? Createdat { get; set; }
    [Display(Name = "Користувач")]
    [Required(ErrorMessage = "Поле 'Користувач' є обов'язковим")]
    public int Userid { get; set; }
    public virtual ICollection<Deckcard>? Deckcards { get; set; } = new List<Deckcard>();
    [Display(Name = "Користувач")]
    public virtual User? User { get; set; }
}
