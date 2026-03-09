
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Cardclass
{
    public int Id { get; set; }
    
    [Display(Name = "Ім'я")]
    [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим")]
    public string Name { get; set; } = null!;
    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Поле 'Опис' є обов'язковим")]
    public string? Description { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}
