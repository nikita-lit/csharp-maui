namespace Example.SnakeGame;

public partial class SnakeGameStart : ContentPage
{
    private Theme _theme;

    public SnakeGameStart()
    {
        InitializeComponent();
        _theme = Theme.LoadSelected();
        UpdateText();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        _theme = Theme.LoadSelected();
        ApplyTheme();
        UpdateText();
        UpdateHighScore();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
    }

    private void ApplyTheme()
    {
        _theme.Apply(this);
        MainLayout.BackgroundColor = _theme.BackgroundColor;
        TitleLabel.TextColor = _theme.TextColor;
        SubtitleLabel.TextColor = _theme.TextColor;
        HighScoreLabel.TextColor = _theme.TextColor;

        PlayButton.BackgroundColor = _theme.ButtonColor;
        PlayIcon.TextColor = _theme.ButtonTextColor;
        PlayText.TextColor = _theme.ButtonTextColor;

        StatsButton.BackgroundColor = _theme.GridColor;
        StatsIcon.TextColor = _theme.TextColor;
        StatsText.TextColor = _theme.TextColor;

        SettingsButton.BackgroundColor = _theme.GridColor;
        SettingsIcon.TextColor = _theme.TextColor;
        SettingsText.TextColor = _theme.TextColor;

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
