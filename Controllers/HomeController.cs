using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using clueGame.Models;
using clueGame.Models.ViewModels;
using clueGame.Services;

namespace clueGame.Controllers;

public class HomeController : Controller
{
    private readonly MongoDbService _mongo;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(MongoDbService mongo, UserManager<ApplicationUser> userManager)
    {
        _mongo = mongo;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.MongoPlayerId is null)
            return RedirectToAction(nameof(Index));

        var player = await _mongo.GetPlayerByIdAsync(user.MongoPlayerId);
        var games  = await _mongo.GetFinishedGamesForPlayerAsync(user.MongoPlayerId);

        var characters = await _mongo.GetAllCharactersAsync();
        var weapons    = await _mongo.GetAllWeaponsAsync();
        var locations  = await _mongo.GetAllLocationsAsync();

        var charMap = characters.ToDictionary(c => c.Id, c => c.Name);
        var weapMap = weapons.ToDictionary(w => w.Id, w => w.Name);
        var locMap  = locations.ToDictionary(l => l.Id, l => l.Name);

        var history = games.Select(g => new GameHistoryEntry
        {
            Date          = g.CreatedAt.ToString("MMM dd, yyyy"),
            Won           = g.WinnerId == user.MongoPlayerId,
            CharacterName = charMap.GetValueOrDefault(g.Secret.CharacterId, "?"),
            WeaponName    = weapMap.GetValueOrDefault(g.Secret.WeaponId, "?"),
            LocationName  = locMap.GetValueOrDefault(g.Secret.LocationId, "?"),
        }).ToList();

        var vm = new DashboardViewModel
        {
            PlayerName      = player?.Name ?? user.UserName ?? "Detective",
            GamesPlayed     = player?.GamesPlayed ?? 0,
            GamesWon        = player?.GamesWon ?? 0,
            AccuracyPercent = player is { GamesPlayed: > 0 }
                                  ? (int)Math.Round(player.WinRate * 100)
                                  : 0,
            History         = history,
        };

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
