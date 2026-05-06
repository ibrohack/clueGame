using System.ComponentModel.DataAnnotations;

namespace clueGame.Models.ViewModels;

/// <summary>
/// ViewModel para el formulario de registro de nuevos jugadores.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 30 caracteres")]
    [Display(Name = "Nombre de usuario")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre visible es obligatorio")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre visible debe tener entre 2 y 50 caracteres")]
    [Display(Name = "Nombre en el juego")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma tu contraseña")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
