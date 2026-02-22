
namespace TmntCardManager.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Deck> Decks { get; set; } = new List<Deck>();

    public virtual Playerprofile? Playerprofile { get; set; }
}
