using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using clueGame.Models;

namespace clueGame.Data;

/// <summary>
/// DbContext para ASP.NET Identity.
/// Gestiona las tablas de autenticación en la base de datos relacional (SQL Server).
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
