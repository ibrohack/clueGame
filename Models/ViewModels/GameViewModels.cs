using clueGame.Models.Mongo;

namespace clueGame.Models.ViewModels;

public class NewGameViewModel
{
    public int BotCount { get; set; } = 3;
    public string SelectedCharacterId { get; set; } = string.Empty;
    public List<MongoCharacter> AvailableCharacters { get; set; } = [];
}

public class DetectiveMatrixRow
{
    public string CardId { get; set; } = string.Empty;
    public string CardName { get; set; } = string.Empty;
    public string RowStatus { get; set; } = "normal";
    public List<PlayerCell> BotCells { get; set; } = [];
}

public class DetectiveMatrixViewModel
{
    public string GameId { get; set; } = string.Empty;
    public List<string> BotNames { get; set; } = [];
    public List<string> BotPlayerIds { get; set; } = [];
    public List<DetectiveMatrixRow> CharacterRows { get; set; } = [];
    public List<DetectiveMatrixRow> WeaponRows { get; set; } = [];
    public List<DetectiveMatrixRow> LocationRows { get; set; } = [];
}

public class BoardViewModel
{
    public MongoGame Game { get; set; } = null!;
    public GamePlayer HumanPlayer { get; set; } = null!;
    public List<MongoCharacter> Characters { get; set; } = [];
    public List<MongoWeapon> Weapons { get; set; } = [];
    public List<MongoLocation> Locations { get; set; } = [];

    // Set after a suggestion is resolved on the same request
    public SuggestionResultViewModel? LastSuggestionResult { get; set; }

    // Set during bot turn step-through
    public BotTurnViewModel? CurrentBotTurn { get; set; }

    // Pending bot index when in bot_turns phase
    public int PendingBotIndex { get; set; }

    public DetectiveMatrixViewModel BuildMatrix()
    {
        var bots = Game.Players
            .Where(p => p.IsBot)
            .OrderBy(p => p.TurnOrder)
            .ToList();

        DetectiveMatrixRow MakeRow(DetectiveCard card, string name) => new()
        {
            CardId = card.Id,
            CardName = name,
            RowStatus = card.Status,
            BotCells = bots.Select(b =>
                card.PlayerCells.FirstOrDefault(c => c.PlayerId == b.PlayerId)
                ?? new PlayerCell { PlayerId = b.PlayerId, CellStatus = "normal" }
            ).ToList()
        };

        return new DetectiveMatrixViewModel
        {
            GameId = Game.Id,
            BotNames = bots.Select(b => b.Name).ToList(),
            BotPlayerIds = bots.Select(b => b.PlayerId).ToList(),
            CharacterRows = HumanPlayer.DetectiveSheet.Characters
                .Select(c => MakeRow(c, Characters.FirstOrDefault(x => x.Id == c.Id)?.Name ?? c.Id))
                .ToList(),
            WeaponRows = HumanPlayer.DetectiveSheet.Weapons
                .Select(w => MakeRow(w, Weapons.FirstOrDefault(x => x.Id == w.Id)?.Name ?? w.Id))
                .ToList(),
            LocationRows = HumanPlayer.DetectiveSheet.Locations
                .Select(l => MakeRow(l, Locations.FirstOrDefault(x => x.Id == l.Id)?.Name ?? l.Id))
                .ToList()
        };
    }
}

public class SuggestionResultViewModel
{
    public string CharacterName { get; set; } = string.Empty;
    public string WeaponName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? RefutedByName { get; set; }
    public string? ShownCardName { get; set; }
    public bool NoOneRefuted { get; set; }
}

public class BotTurnViewModel
{
    public string BotName { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public string WeaponName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? RefutedByName { get; set; }
    public string? ShownCardName { get; set; }
    public bool NoOneRefuted { get; set; }
    public bool HumanMustShowCard { get; set; }
    public string BotPlayerId { get; set; } = string.Empty;
}

public class SuggestViewModel
{
    public string GameId { get; set; } = string.Empty;
    public string CharacterId { get; set; } = string.Empty;
    public string WeaponId { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public List<MongoCharacter> Characters { get; set; } = [];
    public List<MongoWeapon> Weapons { get; set; } = [];
    public List<MongoLocation> Locations { get; set; } = [];
}

public class ShowCardViewModel
{
    public string GameId { get; set; } = string.Empty;
    public string BotPlayerId { get; set; } = string.Empty;
    public int BotIndex { get; set; }
    public List<(string Id, string Name)> MatchingCards { get; set; } = [];
    public string BotName { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public string WeaponName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    // Raw IDs needed to save a complete guess record on submit
    public string SuggestedCharacterId { get; set; } = string.Empty;
    public string SuggestedWeaponId { get; set; } = string.Empty;
    public string SuggestedLocationId { get; set; } = string.Empty;
}

public class AccuseViewModel
{
    public string GameId { get; set; } = string.Empty;
    public string CharacterId { get; set; } = string.Empty;
    public string WeaponId { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public List<MongoCharacter> Characters { get; set; } = [];
    public List<MongoWeapon> Weapons { get; set; } = [];
    public List<MongoLocation> Locations { get; set; } = [];
}

public class GameOverViewModel
{
    public bool Won { get; set; }
    public string SecretCharacterName { get; set; } = string.Empty;
    public string SecretWeaponName { get; set; } = string.Empty;
    public string SecretLocationName { get; set; } = string.Empty;
}
