using System.ComponentModel.DataAnnotations;

namespace TmntCardManager.Api.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Підтвердження пароля є обов'язковим")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролі не співпадають!")]
    public string ConfirmPassword { get; set; } = null!;
}