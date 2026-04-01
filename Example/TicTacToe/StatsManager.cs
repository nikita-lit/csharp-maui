using System.Text.Json;

namespace Example.TicTacToe;

public class GameStats
{
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public int BotWins { get; set; }
    public int Draws { get; set; }
    public List<string> History { get; set; } = new();
}

public static class StatsManager
{
    public static GameStats LoadStats()
    {
        string json = Preferences.Get("TicTacToeStats", "");
        if (string.IsNullOrEmpty(json))
            return new GameStats();

        try
        {
            return JsonSerializer.Deserialize<GameStats>(json) ?? new GameStats();
        }
        catch
        {
            return new GameStats();
        }
    }

    public static void SaveStats(GameStats stats)
    {
        string json = JsonSerializer.Serialize(stats);
        Preferences.Set("TicTacToeStats", json);
    }

    public static void AddWin(string winnerName, bool isBot, string p1Name)
    {
        var stats = LoadStats();
        if (winnerName == p1Name)
            stats.Player1Wins++;
        else if (isBot)
            stats.BotWins++;
        else
            stats.Player2Wins++;

        stats.History.Insert(0, $"{DateTime.Now:dd.MM HH:mm} - Võitja: {winnerName}");
        if (stats.History.Count > 5) 
            stats.History.RemoveAt(5);

        SaveStats(stats);
    }

    public static void AddDraw()
    {
        var stats = LoadStats();
        stats.Draws++;
        stats.History.Insert(0, $"{DateTime.Now:dd.MM HH:mm} - Viik");

        if (stats.History.Count > 5) 
            stats.History.RemoveAt(5);

        SaveStats(stats);
    }
}
