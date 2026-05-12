namespace Example.SnakeGame;

public static class SnakeStatsService
{
    public static event Action? StatsChanged;

    public static int GamesPlayed 
        => Preferences.Get("snake_games_played", 0);
    public static int LastScore 
        => Preferences.Get("snake_last_score", 0);
    public static int BestScore 
        => Preferences.Get("snake_best_score_overall", 0);

    public static void SaveResult(Player player)
    {
        Preferences.Set("snake_games_played", GamesPlayed + 1);
        Preferences.Set("snake_last_score", player.Score);

        if (player.HighScore > BestScore)
            Preferences.Set("snake_best_score_overall", player.HighScore);

        StatsChanged?.Invoke();
    }
}
