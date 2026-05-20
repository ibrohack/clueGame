namespace clueGame.Models.ViewModels;

public class DashboardViewModel
{
    public string PlayerName { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int AccuracyPercent { get; set; }
    public List<GameHistoryEntry> History { get; set; } = [];
}

public class GameHistoryEntry
{
    public string Date { get; set; } = string.Empty;
    public bool Won { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string WeaponName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}
