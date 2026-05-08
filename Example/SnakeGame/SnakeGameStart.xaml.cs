namespace Example.SnakeGame;

public partial class SnakeGameStart : ContentPage
{
    private Theme _theme;

    public SnakeGameStart()
    {
        InitializeComponent();
        _theme = Theme.LoadSelected();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _theme = Theme.LoadSelected();
        ApplyTheme();
        UpdateHighScore();
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

        SettingsButton.BackgroundColor = _theme.GridColor;
        SettingsIcon.TextColor = _theme.TextColor;
        SettingsText.TextColor = _theme.TextColor;
    }

    private void UpdateHighScore()
    {
        string playerName = Preferences.Get("snake_player_name", "Mängija");
        var player = new Player(playerName);
        HighScoreLabel.Text = $"Parim tulemus: {player.HighScore}";
    }

    private async void OnPlayClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new SnakeGamePage());
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new SnakeGameSettings());
    }
}
