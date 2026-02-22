
namespace TmntCardManager.Models;

public class Cardclass
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}
