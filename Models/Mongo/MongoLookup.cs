using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace clueGame.Models.Mongo;

public class MongoCharacter
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [BsonElement("quote")]              public string? Quote { get; set; }
    [BsonElement("temperament")]        public string? Temperament { get; set; }
    [BsonElement("location")]           public string? Location { get; set; }
    [BsonElement("status")]             public string? Status { get; set; }
    [BsonElement("knownAssociates")]    public string? KnownAssociates { get; set; }
    [BsonElement("distinguishingMark")] public string? DistinguishingMark { get; set; }
    [BsonElement("motive")]             public string? Motive { get; set; }
    [BsonElement("description")]        public List<string>? Description { get; set; }
}

public class MongoWeapon
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [BsonElement("quote")]       public string? Quote { get; set; }
    [BsonElement("category")]    public string? Category { get; set; }
    [BsonElement("lethality")]   public string? Lethality { get; set; }
    [BsonElement("origin")]      public string? Origin { get; set; }
    [BsonElement("condition")]   public string? Condition { get; set; }
    [BsonElement("handledBy")]   public string? HandledBy { get; set; }
    [BsonElement("evidence")]    public string? Evidence { get; set; }
    [BsonElement("description")] public List<string>? Description { get; set; }
}

public class MongoLocation
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [BsonElement("quote")]           public string? Quote { get; set; }
    [BsonElement("floor")]           public string? Floor { get; set; }
    [BsonElement("securityLevel")]   public string? SecurityLevel { get; set; }
    [BsonElement("occupancyStatus")] public string? OccupancyStatus { get; set; }
    [BsonElement("knownOccupants")]  public string? KnownOccupants { get; set; }
    [BsonElement("lastIncident")]    public string? LastIncident { get; set; }
    [BsonElement("accessPoints")]    public string? AccessPoints { get; set; }
    [BsonElement("notes")]           public string? Notes { get; set; }
    [BsonElement("description")]     public List<string>? Description { get; set; }
}
