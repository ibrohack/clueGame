using clueGame.Models.Mongo;

namespace clueGame.Services;

public record SuggestionResult(
    string? RefutedByPlayerId,
    string? RefutedByName,
    string? ShownCardId,
    string? ShownCardName
);

public record BotTurnResult(
    string BotName,
    string CharacterId,
    string CharacterName,
    string WeaponId,
    string WeaponName,
    string LocationId,
    string LocationName,
    string? RefutedByName,
    bool HumanMustShowCard,
    List<string> HumanMatchingCards
);

public class GameService
{
    private static readonly Random _rng = new();

    // Builds the full initial game document (does not save to DB).
    public MongoGame SetupGame(
        string humanPlayerId,
        string humanDisplayName,
        int botCount,
        string humanCharacterId,
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
    {
        var allCards = BuildAllCards(characters, weapons, locations);
        Shuffle(allCards);

        // Ensure secret has exactly one of each type
        var secret = PickSecret(characters, weapons, locations);
        var remainingCards = allCards
            .Where(c => c != secret.CharacterId && c != secret.WeaponId && c != secret.LocationId)
            .ToList();
        Shuffle(remainingCards);

        var totalPlayers = 1 + botCount;
        var players = new List<GamePlayer>();

        // Human player
        players.Add(new GamePlayer
        {
            PlayerId = humanPlayerId,
            Name = humanDisplayName,
            CharacterId = humanCharacterId,
            IsBot = false,
            TurnOrder = 1
        });

        // Bots — each gets a remaining character; their display name is the character's name
        var usedCharacters = new List<string> { humanCharacterId };
        for (int i = 0; i < botCount; i++)
        {
            var botCharId = PickRandomCharacterId(characters, usedCharacters);
            usedCharacters.Add(botCharId);
            var botCharName = characters.First(c => c.Id == botCharId).Name;
            players.Add(new GamePlayer
            {
                PlayerId = $"bot_{Guid.NewGuid():N}",
                Name = botCharName,
                CharacterId = botCharId,
                IsBot = true,
                TurnOrder = i + 2
            });
        }

        // Deal cards round-robin
        for (int i = 0; i < remainingCards.Count; i++)
        {
            players[i % totalPlayers].Hand.Add(remainingCards[i]);
        }

        // Build detective sheets for all players
        foreach (var player in players)
        {
            player.DetectiveSheet = BuildDetectiveSheet(
                player, characters, weapons, locations, secret);
        }

        // Randomize turn order (human keeps position 1 for simplicity)
        var game = new MongoGame
        {
            Id = $"game_{Guid.NewGuid():N}",
            CreatedAt = DateTime.UtcNow,
            IsFinished = false,
            Secret = secret,
            Players = players,
            CurrentTurnPlayerId = humanPlayerId,
            TurnPhase = "human_suggestion",
            PendingBotIndex = 0
        };

        return game;
    }

    // Resolves a suggestion: finds the first player (in turn order after guesser)
    // who holds at least one of the suggested cards. Returns null if nobody can refute.
    public SuggestionResult ResolveSuggestion(
        MongoGame game,
        string suggestingPlayerId,
        string characterId,
        string weaponId,
        string locationId,
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
    {
        var suggested = new HashSet<string> { characterId, weaponId, locationId };
        var orderedOthers = game.Players
            .Where(p => p.PlayerId != suggestingPlayerId)
            .OrderBy(p => p.TurnOrder)
            .ToList();

        foreach (var player in orderedOthers)
        {
            var matching = player.Hand.Where(c => suggested.Contains(c)).ToList();
            if (matching.Count == 0) continue;

            // Pick one matching card at random to show
            var shownCardId = matching[_rng.Next(matching.Count)];
            var shownCardName = GetCardName(shownCardId, characters, weapons, locations);

            return new SuggestionResult(player.PlayerId, player.Name, shownCardId, shownCardName);
        }

        return new SuggestionResult(null, null, null, null);
    }

    // Generates a random suggestion for a bot. Prioritises cards the bot doesn't
    // know about yet (unknown on its detective sheet).
    public (string characterId, string weaponId, string locationId) MakeBotSuggestion(
        GamePlayer bot,
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
    {
        var unknownChars = bot.DetectiveSheet.Characters
            .Where(c => c.Status == "unknown").Select(c => c.Id).ToList();
        var unknownWeapons = bot.DetectiveSheet.Weapons
            .Where(c => c.Status == "unknown").Select(c => c.Id).ToList();
        var unknownLocs = bot.DetectiveSheet.Locations
            .Where(c => c.Status == "unknown").Select(c => c.Id).ToList();

        // Fall back to full lists if a category is exhausted
        var charPool = unknownChars.Count > 0 ? unknownChars : characters.Select(c => c.Id).ToList();
        var weapPool = unknownWeapons.Count > 0 ? unknownWeapons : weapons.Select(w => w.Id).ToList();
        var locPool = unknownLocs.Count > 0 ? unknownLocs : locations.Select(l => l.Id).ToList();

        return (
            charPool[_rng.Next(charPool.Count)],
            weapPool[_rng.Next(weapPool.Count)],
            locPool[_rng.Next(locPool.Count)]
        );
    }

    public bool CheckAccusation(MongoGame game, string characterId, string weaponId, string locationId)
        => game.Secret.CharacterId == characterId
        && game.Secret.WeaponId == weaponId
        && game.Secret.LocationId == locationId;

    // After the human shows a card to a bot, mark that card as discarded on the bot's sheet.
    public void ApplyCardShownToBot(MongoGame game, string botPlayerId, string cardId)
    {
        var bot = game.Players.FirstOrDefault(p => p.PlayerId == botPlayerId);
        if (bot is null) return;

        MarkCardOnSheet(bot.DetectiveSheet, cardId, "discarded");
    }

    // After the human receives a card from a bot, mark it on the human's detective sheet.
    public void ApplyCardShownToHuman(MongoGame game, string humanPlayerId, string cardId)
    {
        var human = game.Players.FirstOrDefault(p => p.PlayerId == humanPlayerId);
        if (human is null) return;

        MarkCardOnSheet(human.DetectiveSheet, cardId, "discarded");
    }

    // Returns list of card IDs the human holds that match a suggestion.
    public List<string> GetHumanMatchingCards(GamePlayer human, string characterId, string weaponId, string locationId)
    {
        var suggested = new HashSet<string> { characterId, weaponId, locationId };
        return human.Hand.Where(c => suggested.Contains(c)).ToList();
    }

    // Advances bot detective sheet when a bot-to-bot refutation happens.
    public void ApplyBotToBotRefutation(MongoGame game, string refutingBotId, string shownCardId)
    {
        // Other bots learn that refuting bot has the shown card — mark as discarded for them
        foreach (var player in game.Players.Where(p => p.IsBot && p.PlayerId != refutingBotId))
        {
            MarkCardOnSheet(player.DetectiveSheet, shownCardId, "discarded");
        }
    }

    // Updates a card status on a detective sheet (finds the card in any category).
    public static void MarkCardOnSheet(DetectiveSheet sheet, string cardId, string status)
    {
        var card = sheet.Characters.FirstOrDefault(c => c.Id == cardId)
                ?? sheet.Weapons.FirstOrDefault(c => c.Id == cardId)
                ?? sheet.Locations.FirstOrDefault(c => c.Id == cardId);
        if (card is not null) card.Status = status;
    }

    // --- Helpers ---

    private static GameSecret PickSecret(
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
    {
        return new GameSecret
        {
            CharacterId = characters[_rng.Next(characters.Count)].Id,
            WeaponId = weapons[_rng.Next(weapons.Count)].Id,
            LocationId = locations[_rng.Next(locations.Count)].Id
        };
    }

    private static List<string> BuildAllCards(
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
    {
        var cards = new List<string>();
        cards.AddRange(characters.Select(c => c.Id));
        cards.AddRange(weapons.Select(w => w.Id));
        cards.AddRange(locations.Select(l => l.Id));
        return cards;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static string PickRandomCharacterId(List<MongoCharacter> characters, List<string> excluded)
    {
        var available = characters.Where(c => !excluded.Contains(c.Id)).ToList();
        return available[_rng.Next(available.Count)].Id;
    }

    private static DetectiveSheet BuildDetectiveSheet(
        GamePlayer player,
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations,
        GameSecret secret)
    {
        var sheet = new DetectiveSheet
        {
            Characters = characters.Select(c => new DetectiveCard
            {
                Id = c.Id,
                Status = player.Hand.Contains(c.Id) ? "in_hand" : "unknown"
            }).ToList(),

            Weapons = weapons.Select(w => new DetectiveCard
            {
                Id = w.Id,
                Status = player.Hand.Contains(w.Id) ? "in_hand" : "unknown"
            }).ToList(),

            Locations = locations.Select(l => new DetectiveCard
            {
                Id = l.Id,
                Status = player.Hand.Contains(l.Id) ? "in_hand" : "unknown"
            }).ToList()
        };
        return sheet;
    }

    private static string GetCardName(
        string cardId,
        List<MongoCharacter> characters,
        List<MongoWeapon> weapons,
        List<MongoLocation> locations)
    {
        return characters.FirstOrDefault(c => c.Id == cardId)?.Name
            ?? weapons.FirstOrDefault(w => w.Id == cardId)?.Name
            ?? locations.FirstOrDefault(l => l.Id == cardId)?.Name
            ?? cardId;
    }
}
