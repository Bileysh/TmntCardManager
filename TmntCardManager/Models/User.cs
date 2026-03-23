
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TmntCardManager.Models;

public class User: IdentityUser<int>
{
  public virtual ICollection<Deck>? Decks { get; set; } = new List<Deck>();
  public virtual Playerprofile? Playerprofile { get; set; }
}
