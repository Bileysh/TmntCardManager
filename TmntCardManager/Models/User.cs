
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class User
{
    public int Id { get; set; }
    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage = "Поле 'Електронна пошта' є обов'язковим")]
    [EmailAddress(ErrorMessage = "Невірний формат електронної пошти")]
    public string Email { get; set; } = null!;
    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Поле 'Пароль' є обов'язковим")]
    [MinLength(6, ErrorMessage = "Пaроль повинен бути не менше 6 символів")]
    public string Passwordhash { get; set; } = null!;
    [Display(Name = "Роль")]
    public string? Role { get; set; }

    public virtual ICollection<Deck>? Decks { get; set; } = new List<Deck>();

    public virtual Playerprofile? Playerprofile { get; set; }
}
