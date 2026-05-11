using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace clueGame.Models.Mongo;

public class MongoPlayer
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("avatarUrl")]
    public string AvatarUrl { get; set; } = string.Empty;

    [BsonElement("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [BsonElement("gamesWon")]
    public int GamesWon { get; set; }

    [BsonElement("winRate")]
    public double WinRate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastGameAt")]
    public DateTime? LastGameAt { get; set; }
}
