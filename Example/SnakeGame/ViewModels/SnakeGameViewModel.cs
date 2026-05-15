using Example.SnakeGame.Models;

namespace Example.SnakeGame.ViewModels;

public class SnakeGameViewModel : BaseViewModel
{
    public string ScoreText
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = string.Empty;

    public string HighScoreText
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = string.Empty;

    public static string PlayerName 
        => Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));
    
    public static int Speed 
        => Preferences.Get("snake_speed", 200);

    public static Player CreatePlayer() 
        => new(PlayerName);

    public void UpdateScore(Player player)
    {
        ScoreText = string.Format(LanguageService.Get("SnakeScoreFormat"), player.Score);
        HighScoreText = string.Format(LanguageService.Get("SnakeHighScoreFormat"), player.HighScore);
    }

}
