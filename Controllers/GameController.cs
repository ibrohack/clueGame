using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using clueGame.Models;
using clueGame.Models.Mongo;
using clueGame.Models.ViewModels;
using clueGame.Services;

namespace clueGame.Controllers;

[Authorize]
public class GameController : Controller
{
    private readonly MongoDbService _mongo;
    private readonly GameService _gameService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GameController(
        MongoDbService mongo,
        GameService gameService,
        UserManager<ApplicationUser> userManager)
    {
        _mongo = mongo;
        _gameService = gameService;
        _userManager = userManager;
    }

    // GET /Game
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.MongoPlayerId is null)
            return RedirectToAction("Index", "Home");

        var activeGame = await _mongo.GetActiveGameForPlayerAsync(user.MongoPlayerId);
        if (activeGame is not null)
            return RedirectToAction(nameof(Board), new { gameId = activeGame.Id });

        return RedirectToAction(nameof(New));
    }

    // GET /Game/New
    public async Task<IActionResult> New()
    {
        var characters = await _mongo.GetAllCharactersAsync();
        return View(new NewGameViewModel { AvailableCharacters = characters });
    }

    // POST /Game/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewGameViewModel model)
    {
        var characters = await _mongo.GetAllCharactersAsync();

        if (!ModelState.IsValid || model.BotCount < 2 || model.BotCount > 5)
        {
            model.AvailableCharacters = characters;
            ModelState.AddModelError(string.Empty, "Choose between 2 and 5 bots.");
            return View(nameof(New), model);
        }

        if (string.IsNullOrEmpty(model.SelectedCharacterId) || characters.All(c => c.Id != model.SelectedCharacterId))
        {
            model.AvailableCharacters = characters;
            ModelState.AddModelError(string.Empty, "Please choose a character.");
            return View(nameof(New), model);
        }

        if (characters.Count < model.BotCount + 1)
        {
            model.AvailableCharacters = characters;
            ModelState.AddModelError(string.Empty, "Not enough characters in the database for that many bots.");
            return View(nameof(New), model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user?.MongoPlayerId is null)
            return RedirectToAction("Index", "Home");

        var weapons = await _mongo.GetAllWeaponsAsync();
        var locations = await _mongo.GetAllLocationsAsync();

        var game = _gameService.SetupGame(
            user.MongoPlayerId, user.DisplayName,
            model.BotCount, model.SelectedCharacterId,
            characters, weapons, locations);

        await _mongo.SaveGameAsync(game);

        return RedirectToAction(nameof(Board), new { gameId = game.Id });
    }

    // GET /Game/Board/{gameId}
    public async Task<IActionResult> Board(string gameId)
    {
        var (game, human, characters, weapons, locations) = await LoadGameContext(gameId);
        if (game is null) return RedirectToAction(nameof(New));

        var vm = new BoardViewModel
        {
            Game = game,
            HumanPlayer = human!,
            Characters = characters,
            Weapons = weapons,
            Locations = locations,
            PendingBotIndex = game.PendingBotIndex
        };

        // Restore TempData messages
        if (TempData["LastSuggestionResult"] is string suggestJson)
            vm.LastSuggestionResult = System.Text.Json.JsonSerializer
                .Deserialize<SuggestionResultViewModel>(suggestJson);

        if (TempData["CurrentBotTurn"] is string botJson)
            vm.CurrentBotTurn = System.Text.Json.JsonSerializer
                .Deserialize<BotTurnViewModel>(botJson);

        return View(vm);
    }

    // POST /Game/Suggest
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Suggest(SuggestViewModel model)
    {
        var (game, human, characters, weapons, locations) = await LoadGameContext(model.GameId);
        if (game is null || human is null) return RedirectToAction(nameof(New));

        if (game.TurnPhase != "human_suggestion")
            return RedirectToAction(nameof(Board), new { gameId = model.GameId });

        var result = _gameService.ResolveSuggestion(
            game, human.PlayerId,
            model.CharacterId, model.WeaponId, model.LocationId,
            characters, weapons, locations);

        // Record guess
        await _mongo.SaveGuessAsync(new MongoGuess
        {
            GameId = game.Id,
            PlayerId = human.PlayerId,
            CharacterId = model.CharacterId,
            WeaponId = model.WeaponId,
            LocationId = model.LocationId,
            RefutedByPlayerId = result.RefutedByPlayerId,
            ShownCardId = result.ShownCardId
        });

        // Auto-update human's detective sheet matrix
        if (result.RefutedByPlayerId is not null && result.ShownCardId is not null)
            _gameService.ApplyHumanSuggestionRefuted(
                game, human.PlayerId,
                result.RefutedByPlayerId, result.ShownCardId,
                model.CharacterId, model.WeaponId, model.LocationId);
        else
            _gameService.ApplyNoRefutationOutcome(
                game, human.PlayerId,
                model.CharacterId, model.WeaponId, model.LocationId);

        // Move to "end of human turn" — waiting for EndTurn
        game.TurnPhase = "human_done";
        await _mongo.SaveGameAsync(game);

        var charName = characters.First(c => c.Id == model.CharacterId).Name;
        var weapName = weapons.First(w => w.Id == model.WeaponId).Name;
        var locName = locations.First(l => l.Id == model.LocationId).Name;

        TempData["LastSuggestionResult"] = System.Text.Json.JsonSerializer.Serialize(
            new SuggestionResultViewModel
            {
                CharacterName = charName,
                WeaponName = weapName,
                LocationName = locName,
                RefutedByName = result.RefutedByName,
                ShownCardName = result.ShownCardName,
                NoOneRefuted = result.RefutedByPlayerId is null
            });

        return RedirectToAction(nameof(Board), new { gameId = model.GameId });
    }

    // POST /Game/EndTurn — human ends their turn, starts bot turns
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EndTurn(string gameId)
    {
        var (game, human, _, _, _) = await LoadGameContext(gameId);
        if (game is null || human is null) return RedirectToAction(nameof(New));

        game.TurnPhase = "bot_turns";
        game.PendingBotIndex = 0;
        await _mongo.SaveGameAsync(game);

        // Redirect to Board; the "Next Bot Turn" button there will POST to NextBot.
        return RedirectToAction(nameof(Board), new { gameId });
    }

    // POST /Game/NextBot — advance one bot turn
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NextBot(string gameId, int botIndex)
    {
        var (game, human, characters, weapons, locations) = await LoadGameContext(gameId);
        if (game is null || human is null) return RedirectToAction(nameof(New));

        var bots = game.Players.Where(p => p.IsBot).OrderBy(p => p.TurnOrder).ToList();

        if (botIndex >= bots.Count)
        {
            // All bot turns done — back to human turn
            game.TurnPhase = "human_suggestion";
            game.PendingBotIndex = 0;
            await _mongo.SaveGameAsync(game);
            return RedirectToAction(nameof(Board), new { gameId });
        }

        var bot = bots[botIndex];
        var (charId, weapId, locId) = _gameService.MakeBotSuggestion(bot, characters, weapons, locations);

        var result = _gameService.ResolveSuggestion(
            game, bot.PlayerId, charId, weapId, locId,
            characters, weapons, locations);

        var charName = characters.First(c => c.Id == charId).Name;
        var weapName = weapons.First(w => w.Id == weapId).Name;
        var locName = locations.First(l => l.Id == locId).Name;

        // Human must show a card to this bot
        if (result.RefutedByPlayerId == human.PlayerId)
        {
            var matchingCards = _gameService.GetHumanMatchingCards(human, charId, weapId, locId);
            var cardNames = matchingCards
                .Select(id => (id, GetCardName(id, characters, weapons, locations)))
                .ToList();

            game.PendingBotIndex = botIndex;
            await _mongo.SaveGameAsync(game);

            var showVm = new ShowCardViewModel
            {
                GameId = gameId,
                BotPlayerId = bot.PlayerId,
                BotIndex = botIndex,
                MatchingCards = cardNames,
                BotName = bot.Name,
                CharacterName = charName,
                WeaponName = weapName,
                LocationName = locName,
                SuggestedCharacterId = charId,
                SuggestedWeaponId = weapId,
                SuggestedLocationId = locId
            };
            return View("ShowCard", showVm);
        }

        // Bot-to-bot or nobody refuted
        await _mongo.SaveGuessAsync(new MongoGuess
        {
            GameId = game.Id,
            PlayerId = bot.PlayerId,
            CharacterId = charId,
            WeaponId = weapId,
            LocationId = locId,
            RefutedByPlayerId = result.RefutedByPlayerId,
            ShownCardId = result.ShownCardId
        });

        if (result.ShownCardId is not null && result.RefutedByPlayerId is not null)
            _gameService.ApplyBotToBotRefutation(
                game, human.PlayerId,
                bot.PlayerId, result.RefutedByPlayerId,
                result.ShownCardId,
                charId, weapId, locId);
        else
            _gameService.ApplyNoRefutationOutcome(
                game, human.PlayerId,
                charId, weapId, locId);

        game.PendingBotIndex = botIndex + 1;
        await _mongo.SaveGameAsync(game);

        TempData["CurrentBotTurn"] = System.Text.Json.JsonSerializer.Serialize(
            new BotTurnViewModel
            {
                BotName = bot.Name,
                CharacterName = charName,
                WeaponName = weapName,
                LocationName = locName,
                RefutedByName = result.RefutedByName,
                ShownCardName = result.RefutedByPlayerId != human.PlayerId ? result.ShownCardName : null,
                NoOneRefuted = result.RefutedByPlayerId is null,
                HumanMustShowCard = false,
                BotPlayerId = bot.PlayerId
            });

        return RedirectToAction(nameof(Board), new { gameId });
    }

    // POST /Game/ShowCard — human reveals a card to a bot
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ShowCard(
        string gameId, string botPlayerId, int botIndex, string cardId,
        string suggestedCharacterId, string suggestedWeaponId, string suggestedLocationId)
    {
        var (game, human, characters, weapons, locations) = await LoadGameContext(gameId);
        if (game is null || human is null) return RedirectToAction(nameof(New));

        var bot = game.Players.FirstOrDefault(p => p.PlayerId == botPlayerId);
        if (bot is null) return RedirectToAction(nameof(Board), new { gameId });

        _gameService.ApplyCardShownToBot(game, botPlayerId, cardId);
        _gameService.ApplyHumanRefutedBotSuggestion(
            game, human.PlayerId, botPlayerId,
            suggestedCharacterId, suggestedWeaponId, suggestedLocationId);

        await _mongo.SaveGuessAsync(new MongoGuess
        {
            GameId = game.Id,
            PlayerId = bot.PlayerId,
            CharacterId = suggestedCharacterId,
            WeaponId = suggestedWeaponId,
            LocationId = suggestedLocationId,
            RefutedByPlayerId = human.PlayerId,
            ShownCardId = cardId
        });

        game.PendingBotIndex = botIndex + 1;
        await _mongo.SaveGameAsync(game);

        var charName = characters.FirstOrDefault(c => c.Id == suggestedCharacterId)?.Name ?? string.Empty;
        var weapName = weapons.FirstOrDefault(w => w.Id == suggestedWeaponId)?.Name ?? string.Empty;
        var locName = locations.FirstOrDefault(l => l.Id == suggestedLocationId)?.Name ?? string.Empty;

        TempData["CurrentBotTurn"] = System.Text.Json.JsonSerializer.Serialize(
            new BotTurnViewModel
            {
                BotName = bot.Name,
                CharacterName = charName,
                WeaponName = weapName,
                LocationName = locName,
                RefutedByName = "You",
                NoOneRefuted = false,
                HumanMustShowCard = false,
                BotPlayerId = botPlayerId
            });

        // Redirect to Board; the "Next Bot Turn" button there will POST to NextBot.
        return RedirectToAction(nameof(Board), new { gameId });
    }

    // GET /Game/Accuse
    public async Task<IActionResult> Accuse(string gameId)
    {
        var (_, _, characters, weapons, locations) = await LoadGameContext(gameId);
        return View(new AccuseViewModel
        {
            GameId = gameId,
            Characters = characters,
            Weapons = weapons,
            Locations = locations
        });
    }

    // POST /Game/Accuse
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Accuse")]
    public async Task<IActionResult> AccusePost(AccuseViewModel model)
    {
        var (game, human, characters, weapons, locations) = await LoadGameContext(model.GameId);
        if (game is null || human is null) return RedirectToAction(nameof(New));

        var won = _gameService.CheckAccusation(game, model.CharacterId, model.WeaponId, model.LocationId);

        game.IsFinished = true;
        game.WinnerId = won ? human.PlayerId : null;
        await _mongo.SaveGameAsync(game);

        await _mongo.SaveGuessAsync(new MongoGuess
        {
            GameId = game.Id,
            PlayerId = human.PlayerId,
            CharacterId = model.CharacterId,
            WeaponId = model.WeaponId,
            LocationId = model.LocationId,
            IsCorrect = won
        });

        if (human.PlayerId is not null)
            await _mongo.UpdatePlayerStatsAsync(human.PlayerId, won);

        var overVm = new GameOverViewModel
        {
            Won = won,
            SecretCharacterName = characters.First(c => c.Id == game.Secret.CharacterId).Name,
            SecretWeaponName = weapons.First(w => w.Id == game.Secret.WeaponId).Name,
            SecretLocationName = locations.First(l => l.Id == game.Secret.LocationId).Name
        };

        TempData["GameOver"] = System.Text.Json.JsonSerializer.Serialize(overVm);
        return RedirectToAction(nameof(Over));
    }

    // GET /Game/Over
    public IActionResult Over()
    {
        if (TempData["GameOver"] is not string json)
            return RedirectToAction(nameof(New));

        var vm = System.Text.Json.JsonSerializer.Deserialize<GameOverViewModel>(json);
        return View(vm);
    }

    // POST /Game/Abandon
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Abandon(string gameId)
    {
        var (game, _, _, _, _) = await LoadGameContext(gameId);
        if (game is not null)
        {
            game.IsFinished = true;
            await _mongo.SaveGameAsync(game);
        }
        return RedirectToAction(nameof(New));
    }

    // POST /Game/ToggleCardStatus — lightweight status toggle from detective sheet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCardStatus(string gameId, string cardId, string status)
    {
        var (game, human, _, _, _) = await LoadGameContext(gameId);
        if (game is null || human is null) return BadRequest();

        var allowed = new[] { "normal", "suspect", "discarded" };
        if (!allowed.Contains(status)) return BadRequest();

        // Don't allow changing "in_hand" cards
        var card = human.DetectiveSheet.Characters.FirstOrDefault(c => c.Id == cardId)
                ?? human.DetectiveSheet.Weapons.FirstOrDefault(c => c.Id == cardId)
                ?? human.DetectiveSheet.Locations.FirstOrDefault(c => c.Id == cardId);

        if (card is not null && card.Status != "in_hand")
        {
            card.Status = status;
            await _mongo.SaveGameAsync(game);
        }

        return RedirectToAction(nameof(Board), new { gameId });
    }

    // POST /Game/ToggleCellStatus — human updates a per-bot cell on the detective sheet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCellStatus(
        string gameId, string cardId, string botPlayerId, string cellStatus)
    {
        var (game, human, _, _, _) = await LoadGameContext(gameId);
        if (game is null || human is null) return BadRequest();

        var allowedCellStatuses = new[] { "normal", "has_it", "no_has_it", "suspect_has", "suspect_no" };
        if (!allowedCellStatuses.Contains(cellStatus)) return BadRequest();

        var bot = game.Players.FirstOrDefault(p => p.PlayerId == botPlayerId && p.IsBot);
        if (bot is null) return BadRequest();

        var card = GameService.FindCardOnSheet(human.DetectiveSheet, cardId);
        if (card is null) return BadRequest();

        var cell = card.PlayerCells.FirstOrDefault(c => c.PlayerId == botPlayerId);
        if (cell is not null)
        {
            cell.CellStatus = cellStatus;
            await _mongo.SaveGameAsync(game);
        }

        return RedirectToAction(nameof(Board), new { gameId });
    }

    // --- Private helpers ---

    private async Task<(MongoGame? game, GamePlayer? human,
        List<MongoCharacter> chars, List<MongoWeapon> weapons, List<MongoLocation> locs)>
        LoadGameContext(string gameId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.MongoPlayerId is null)
            return (null, null, [], [], []);

        var game = await _mongo.GetGameByIdAsync(gameId);
        if (game is null)
            return (null, null, [], [], []);

        var human = game.Players.FirstOrDefault(p => p.PlayerId == user.MongoPlayerId && !p.IsBot);

        var characters = await _mongo.GetAllCharactersAsync();
        var weapons = await _mongo.GetAllWeaponsAsync();
        var locations = await _mongo.GetAllLocationsAsync();

        return (game, human, characters, weapons, locations);
    }

    private static string GetCardName(
        string cardId,
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
        => characters.FirstOrDefault(c => c.Id == cardId)?.Name
        ?? weapons.FirstOrDefault(w => w.Id == cardId)?.Name
        ?? locations.FirstOrDefault(l => l.Id == cardId)?.Name
        ?? cardId;
}
