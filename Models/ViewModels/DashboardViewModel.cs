namespace clueGame.Models.ViewModels;

public class DashboardViewModel
{
    public string PlayerName { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int AccuracyPercent { get; set; }
    public int CurrentStreak { get; set; }
    public int TotalClues { get; set; }
    public string MostPlayedCharacter { get; set; } = string.Empty;
    public string LastGameDate { get; set; } = string.Empty;
    public List<GameHistoryEntry> History { get; set; } = [];
}

public class GameHistoryEntry
{
    public string Date { get; set; } = string.Empty;
    public bool Won { get; set; }
    public string PlayedAsCharacter { get; set; } = string.Empty;
    public int NumPlayers { get; set; }
    public string SecretCharacter { get; set; } = string.Empty;
    public string SecretWeapon { get; set; } = string.Empty;
    public string SecretLocation { get; set; } = string.Empty;
    // Keep old names as aliases so existing view code compiles
    public string CharacterName { get => SecretCharacter; set => SecretCharacter = value; }
    public string WeaponName    { get => SecretWeapon;    set => SecretWeapon = value; }
    public string LocationName  { get => SecretLocation;  set => SecretLocation = value; }
}
