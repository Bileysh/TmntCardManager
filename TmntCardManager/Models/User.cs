
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TmntCardManager.Models;

public class User: IdentityUser<int>
{
  public virtual ICollection<Deck>? Decks { get; set; } = new List<Deck>();
  public virtual Playerprofile? Playerprofile { get; set; }
  public virtual ICollection<UserCard>? Usercards { get; set; } = new List<UserCard>();
  public virtual ICollection<Transaction>? Transactions { get; set; } = new List<Transaction>();
  public virtual ICollection<Match> MatchesAsPlayer1 { get; set; } = new List<Match>();
  public virtual ICollection<Match> MatchesAsPlayer2 { get; set; } = new List<Match>();
  public virtual ICollection<Match> MatchesAsWinner { get; set; } = new List<Match>();
}
