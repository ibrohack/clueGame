using MongoDB.Driver;
using clueGame.Models.Mongo;

namespace clueGame.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _db;

    public MongoDbService(IConfiguration config)
    {
        var connectionString = config["MongoDB:ConnectionString"]!;
        var databaseName = config["MongoDB:DatabaseName"]!;
        var client = new MongoClient(connectionString);
        _db = client.GetDatabase(databaseName);
    }

    internal IMongoDatabase Database => _db;

    // --- Lookup collections (read-only master data) ---

    public async Task<List<MongoCharacter>> GetAllCharactersAsync()
        => await _db.GetCollection<MongoCharacter>("characters")
                    .Find(_ => true).ToListAsync();

    public async Task<List<MongoWeapon>> GetAllWeaponsAsync()
        => await _db.GetCollection<MongoWeapon>("weapons")
                    .Find(_ => true).ToListAsync();

    public async Task<List<MongoLocation>> GetAllLocationsAsync()
        => await _db.GetCollection<MongoLocation>("locations")
                    .Find(_ => true).ToListAsync();

    public async Task<MongoCharacter?> GetCharacterByIdAsync(string id)
        => await _db.GetCollection<MongoCharacter>("characters")
                    .Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<MongoWeapon?> GetWeaponByIdAsync(string id)
        => await _db.GetCollection<MongoWeapon>("weapons")
                    .Find(w => w.Id == id).FirstOrDefaultAsync();

    public async Task<MongoLocation?> GetLocationByIdAsync(string id)
        => await _db.GetCollection<MongoLocation>("locations")
                    .Find(l => l.Id == id).FirstOrDefaultAsync();

    // --- Players ---

    public async Task<MongoPlayer> CreatePlayerAsync(string displayName)
    {
        var player = new MongoPlayer
        {
            Id = $"player_{Guid.NewGuid():N}",
            Name = displayName,
            AvatarUrl = "/img/avatars/default.png",
            CreatedAt = DateTime.UtcNow
        };
        await _db.GetCollection<MongoPlayer>("players").InsertOneAsync(player);
        return player;
    }

    public async Task<MongoPlayer?> GetPlayerByIdAsync(string id)
        => await _db.GetCollection<MongoPlayer>("players")
                    .Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task UpdatePlayerStatsAsync(string playerId, bool won)
    {
        var col = _db.GetCollection<MongoPlayer>("players");
        var player = await col.Find(p => p.Id == playerId).FirstOrDefaultAsync();
        if (player is null) return;

        player.GamesPlayed++;
        if (won) player.GamesWon++;
        player.WinRate = player.GamesPlayed > 0
            ? (double)player.GamesWon / player.GamesPlayed
            : 0;
        player.LastGameAt = DateTime.UtcNow;

        await col.ReplaceOneAsync(p => p.Id == playerId, player);
    }

    // --- Games ---

    public async Task<MongoGame> SaveGameAsync(MongoGame game)
    {
        var col = _db.GetCollection<MongoGame>("games");
        var exists = await col.Find(g => g.Id == game.Id).AnyAsync();
        if (exists)
            await col.ReplaceOneAsync(g => g.Id == game.Id, game);
        else
            await col.InsertOneAsync(game);
        return game;
    }

    public async Task<MongoGame?> GetActiveGameForPlayerAsync(string playerId)
        => await _db.GetCollection<MongoGame>("games")
                    .Find(g => !g.IsFinished && g.Players.Any(p => p.PlayerId == playerId && !p.IsBot))
                    .FirstOrDefaultAsync();

    public async Task<MongoGame?> GetGameByIdAsync(string gameId)
        => await _db.GetCollection<MongoGame>("games")
                    .Find(g => g.Id == gameId).FirstOrDefaultAsync();

    // --- Guesses ---

    public async Task SaveGuessAsync(MongoGuess guess)
    {
        guess.Id = $"guess_{Guid.NewGuid():N}";
        await _db.GetCollection<MongoGuess>("guesses").InsertOneAsync(guess);
    }
}
