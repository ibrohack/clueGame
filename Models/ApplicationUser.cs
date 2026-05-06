using Microsoft.AspNetCore.Identity;

namespace clueGame.Models;

/// <summary>
/// Modelo de usuario personalizado que extiende IdentityUser.
/// Contiene campos adicionales específicos del juego CLUE.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nombre visible en el juego (diferente del UserName de login).
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// ID del documento Player en MongoDB.
    /// Se asigna tras el registro para vincular Identity con MongoDB.
    /// </summary>
    public string? MongoPlayerId { get; set; }

    /// <summary>
    /// Fecha de registro del usuario.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
