using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace clueGame.Models.Mongo;

public class MongoGuess
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("gameId")]
    public string GameId { get; set; } = string.Empty;

    [BsonElement("playerId")]
    public string PlayerId { get; set; } = string.Empty;

    [BsonElement("characterId")]
    public string CharacterId { get; set; } = string.Empty;

    [BsonElement("weaponId")]
    public string WeaponId { get; set; } = string.Empty;

    [BsonElement("locationId")]
    public string LocationId { get; set; } = string.Empty;

    [BsonElement("refutedByPlayerId")]
    public string? RefutedByPlayerId { get; set; }

    [BsonElement("shownCardId")]
    public string? ShownCardId { get; set; }

    [BsonElement("isCorrect")]
    public bool IsCorrect { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
