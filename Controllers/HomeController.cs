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

        var pid = user.MongoPlayerId;

        // Run all independent queries in parallel
        var playerTask   = _mongo.GetPlayerByIdAsync(pid);
        var histTask     = _mongo.GetFinishedGamesForPlayerAsync(pid, 10);
        var allGamesTask = _mongo.GetAllFinishedGamesForPlayerAsync(pid);
        var cluesTask    = _mongo.CountAllPlayerGuessesAsync(pid);
        var charsTask    = _mongo.GetAllCharactersAsync();
        var weapTask     = _mongo.GetAllWeaponsAsync();
        var locTask      = _mongo.GetAllLocationsAsync();

        await Task.WhenAll(playerTask, histTask, allGamesTask, cluesTask, charsTask, weapTask, locTask);

        var player     = playerTask.Result;
        var histGames  = histTask.Result;
        var allGames   = allGamesTask.Result;
        var totalClues = cluesTask.Result;
        var characters = charsTask.Result;
        var weapons    = weapTask.Result;
        var locations  = locTask.Result;

        var charMap = characters.ToDictionary(c => c.Id, c => c.Name);
        var weapMap = weapons.ToDictionary(w => w.Id, w => w.Name);
        var locMap  = locations.ToDictionary(l => l.Id, l => l.Name);

        // Authoritative stats derived directly from games (not from stale player counters)
        int gamesPlayed = allGames.Count;
        int gamesWon    = allGames.Count(g => g.WinnerId == pid);
        int accuracy    = gamesPlayed > 0
                              ? (int)Math.Round((double)gamesWon / gamesPlayed * 100)
                              : 0;

        // Current win streak (consecutive wins from most recent backwards)
        int streak = 0;
        foreach (var g in allGames)   // already sorted descending by CreatedAt
        {
            if (g.WinnerId == pid) streak++;
            else break;
        }

        // Most frequently played character
        var topChar = allGames
            .Select(g => g.Players.FirstOrDefault(p => p.PlayerId == pid && !p.IsBot)?.CharacterId)
            .Where(id => id is not null)
            .GroupBy(id => id!)
            .OrderByDescending(grp => grp.Count())
            .FirstOrDefault();
        var mostPlayedCharacter = topChar is not null
            ? charMap.GetValueOrDefault(topChar.Key, "?")
            : string.Empty;

        // Last game date
        var lastGame     = allGames.FirstOrDefault();   // sorted descending already
        var lastGameDate = lastGame is not null
            ? lastGame.CreatedAt.ToString("MMM dd, yyyy")
            : "—";

        // History rows (last 10 games)
        var history = histGames.Select(g =>
        {
            var humanSlot = g.Players.FirstOrDefault(p => p.PlayerId == pid && !p.IsBot);
            return new GameHistoryEntry
            {
                Date              = g.CreatedAt.ToString("MMM dd, yyyy"),
                Won               = g.WinnerId == pid,
                PlayedAsCharacter = charMap.GetValueOrDefault(humanSlot?.CharacterId ?? "", "?"),
                NumPlayers        = g.Players.Count,
                SecretCharacter   = charMap.GetValueOrDefault(g.Secret.CharacterId, "?"),
                SecretWeapon      = weapMap.GetValueOrDefault(g.Secret.WeaponId, "?"),
                SecretLocation    = locMap.GetValueOrDefault(g.Secret.LocationId, "?"),
            };
        }).ToList();

        var vm = new DashboardViewModel
        {
            PlayerName          = player?.Name ?? user.UserName ?? "Detective",
            GamesPlayed         = gamesPlayed,
            GamesWon            = gamesWon,
            AccuracyPercent     = accuracy,
            CurrentStreak       = streak,
            TotalClues          = (int)totalClues,
            MostPlayedCharacter = mostPlayedCharacter,
            LastGameDate        = lastGameDate,
            History             = history,
        };

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
