using System.ComponentModel.DataAnnotations;

namespace clueGame.Models.ViewModels;

/// <summary>
/// ViewModel para el formulario de inicio de sesión.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario o email es obligatorio")]
    [Display(Name = "Usuario o Email")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}
