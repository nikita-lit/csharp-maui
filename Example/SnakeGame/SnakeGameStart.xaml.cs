using Example.SnakeGame.Models;

namespace Example.SnakeGame;

public partial class SnakeGameStart : ContentPage
{
    public SnakeGameStart()
    {
        InitializeComponent();
        
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        
        UpdateText();
        UpdateHighScore();
        Theme.Current.Apply(this);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateText();
        UpdateHighScore();
        Theme.Current.Apply(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeStartTitle");
        PlayText.Text = LanguageService.Get("SnakePlayGame");
        StatsText.Text = LanguageService.Get("SnakeStats");
        SettingsText.Text = LanguageService.Get("SnakeSettings");
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        var playerName = Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));
        var player = new Player(playerName);
        HighScoreLabel.Text = string.Format(LanguageService.Get("SnakeBestScoreFormat"), player.HighScore);
    }

    private async void OnPlayClicked(object? sender, EventArgs e) 
        => await Navigation.PushAsync(new SnakeGamePage());
    private async void OnStatsClicked(object? sender, EventArgs e) 
        => await Navigation.PushAsync(new SnakeGameStats());

    private async void OnSettingsClicked(object? sender, EventArgs e) 
        =>  await Navigation.PushAsync(new SnakeGameSettings());
}
