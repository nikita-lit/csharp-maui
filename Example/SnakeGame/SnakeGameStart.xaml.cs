namespace Example.SnakeGame;

public partial class SnakeGameStart : ContentPage
{
    public SnakeGameStart()
    {
        InitializeComponent();
        UpdateText();
        ApplyTheme(Theme.Current);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        UpdateText();
        UpdateHighScore();
        ApplyTheme(Theme.Current);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
    }

    private void ApplyTheme(Theme theme)
    {
        BackgroundColor = theme.BackgroundColor;
        MainLayout.BackgroundColor = theme.BackgroundColor;
        TitleLabel.TextColor = theme.TextColor;
        SubtitleLabel.TextColor = theme.TextColor;
        HighScoreLabel.TextColor = theme.TextColor;

        PlayButton.BackgroundColor = theme.ButtonColor;
        PlayIcon.TextColor = theme.ButtonTextColor;
        PlayText.TextColor = theme.ButtonTextColor;

        StatsButton.BackgroundColor = theme.GridColor;
        StatsIcon.TextColor = theme.TextColor;
        StatsText.TextColor = theme.TextColor;

        SettingsButton.BackgroundColor = theme.GridColor;
        SettingsIcon.TextColor = theme.TextColor;
        SettingsText.TextColor = theme.TextColor;

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
