
namespace TmntCardManager.Models;

public class Deck
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public int Userid { get; set; }

    public virtual ICollection<Deckcard> Deckcards { get; set; } = new List<Deckcard>();

    public virtual User User { get; set; } = null!;
}
