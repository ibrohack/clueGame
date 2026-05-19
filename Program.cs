using clueGame.Data;
using clueGame.Models;
using clueGame.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<GameService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});




// Configurar ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Política de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Política de usuario
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar rutas de cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed lore data into MongoDB (idempotent — skips already-seeded documents)
using (var scope = app.Services.CreateScope())
{
    var mongo = scope.ServiceProvider.GetRequiredService<MongoDbService>();
    await new clueGame.Services.LoreSeeder(mongo).SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

// Catalog detail pages — must be registered before the default route
app.MapControllerRoute("catalog-character", "Characters/{id}", new { controller = "Catalog", action = "Character" });
app.MapControllerRoute("catalog-weapon",    "Weapons/{id}",    new { controller = "Catalog", action = "Weapon" });
app.MapControllerRoute("catalog-location",  "Locations/{id}",  new { controller = "Catalog", action = "Location" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();
