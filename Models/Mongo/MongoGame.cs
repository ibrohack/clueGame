using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace clueGame.Models.Mongo;

public class MongoGame
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("isFinished")]
    public bool IsFinished { get; set; }

    [BsonElement("winnerId")]
    public string? WinnerId { get; set; }

    [BsonElement("secret")]
    public GameSecret Secret { get; set; } = new();

    [BsonElement("players")]
    public List<GamePlayer> Players { get; set; } = [];

    [BsonElement("currentTurnPlayerId")]
    public string CurrentTurnPlayerId { get; set; } = string.Empty;

    // Phase within the current human turn:
    // "human_suggestion" | "bot_turns" | "finished"
    [BsonElement("turnPhase")]
    public string TurnPhase { get; set; } = "human_suggestion";

    // Which bot index is next during bot turns (0-based among bots list)
    [BsonElement("pendingBotIndex")]
    public int PendingBotIndex { get; set; }
}

public class GameSecret
{
    [BsonElement("characterId")]
    public string CharacterId { get; set; } = string.Empty;

    [BsonElement("weaponId")]
    public string WeaponId { get; set; } = string.Empty;

    [BsonElement("locationId")]
    public string LocationId { get; set; } = string.Empty;
}

public class GamePlayer
{
    [BsonElement("playerId")]
    public string PlayerId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("characterId")]
    public string CharacterId { get; set; } = string.Empty;

    [BsonElement("isBot")]
    public bool IsBot { get; set; }

    [BsonElement("turnOrder")]
    public int TurnOrder { get; set; }

    [BsonElement("hand")]
    public List<string> Hand { get; set; } = [];

    [BsonElement("detectiveSheet")]
    public DetectiveSheet DetectiveSheet { get; set; } = new();
}

public class DetectiveSheet
{
    [BsonElement("characters")]
    public List<DetectiveCard> Characters { get; set; } = [];

    [BsonElement("weapons")]
    public List<DetectiveCard> Weapons { get; set; } = [];

    [BsonElement("locations")]
    public List<DetectiveCard> Locations { get; set; } = [];
}

public class DetectiveCard
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    // "unknown" | "in_hand" | "discarded" | "suspect"
    [BsonElement("status")]
    public string Status { get; set; } = "unknown";
}
