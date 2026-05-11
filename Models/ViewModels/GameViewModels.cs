using clueGame.Models.Mongo;

namespace clueGame.Models.ViewModels;

public class NewGameViewModel
{
    public int BotCount { get; set; } = 3;
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
