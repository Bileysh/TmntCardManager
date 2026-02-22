
namespace TmntCardManager.Models;

public class Deckcard
{
    public int Id { get; set; }

    public int Deckid { get; set; }

    public int Quantity { get; set; }

    public int Cardid { get; set; }

    public virtual Card Card { get; set; } = null!;

    public virtual Deck Deck { get; set; } = null!;
}
