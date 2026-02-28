
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Cardclass
{
    public int Id { get; set; }
    
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;
    [Display(Name = "Опис")]
    public string? Description { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}
