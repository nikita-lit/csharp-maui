using Example.SnakeGame.Services;

namespace Example.SnakeGame.ViewModels;

public class SnakeStatsViewModel : BaseViewModel
{
    public string PlayerNameText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string GamesPlayedText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string LastScoreText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string BestScoreText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public void Refresh()
    {
        var playerName = Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));
        var player = new Player(playerName);

        PlayerNameText = string.Format(LanguageService.Get("SnakeStatsPlayerFormat"), playerName);
        GamesPlayedText = string.Format(LanguageService.Get("SnakeStatsGamesFormat"), StatsService.GamesPlayed);
        LastScoreText = string.Format(LanguageService.Get("SnakeStatsLastScoreFormat"), StatsService.LastScore);
        BestScoreText = string.Format(LanguageService.Get("SnakeStatsBestScoreFormat"), Math.Max(player.HighScore, StatsService.BestScore));
    }
}
