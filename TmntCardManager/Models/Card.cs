
using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Models;

public class Card
{
    public int Id { get; set; }

    [Display(Name = "Зображення")]
    [Required(ErrorMessage = "Поле 'Зображення' є обов'язковим")]
    public string? Imageurl { get; set; }

    [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим")]
    [Display(Name = "Ім'я")]
    public string? Name { get; set; }
    
    [Range(0, 9999, ErrorMessage = "Значення має бути від 0 до 9999")]
    [Display(Name = "Сила")]
    public int? Strength { get; set; }
    
    [Range(0, 9999, ErrorMessage = "Значення має бути від 0 до 9999")]
    [Display(Name = "Спритність")]
    public int? Agility { get; set; }
    
    [Range(0, 9999, ErrorMessage = "Значення має бути від 0 до 9999")]
    [Display(Name = "Майстерність")]
    public int? Skill { get; set; }
    
    [Range(0, 9999, ErrorMessage = "Значення має бути від 0 до 9999")]
    [Display(Name = "Кмітливість")]
    public int? Wit { get; set; }
    [Display(Name = "Фракція")]
    public int? Classid { get; set; }
    [Display(Name = "Фракція")]
    public virtual Cardclass Class { get; set; }

    public virtual ICollection<Deckcard> Deckcards { get; set; } = new List<Deckcard>();
    public virtual ICollection<UserCard> Usercards { get; set; } = new List<UserCard>();
}
