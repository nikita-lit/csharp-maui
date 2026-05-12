using System.Globalization;

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

        SettingsButton.BackgroundColor = _theme.GridColor;
        SettingsIcon.TextColor = _theme.TextColor;
        SettingsText.TextColor = _theme.TextColor;

        LanguageButton.BackgroundColor = _theme.ButtonColor;
        LanguageButton.TextColor = _theme.ButtonTextColor;
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeStartTitle");
        PlayText.Text = LanguageService.Get("SnakePlayGame");
        SettingsText.Text = LanguageService.Get("SnakeSettings");
        LanguageButton.Text = LanguageService.Get("LanguageButton");
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        var playerName = Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));
        var player = new Player(playerName);
        HighScoreLabel.Text = string.Format(LanguageService.Get("SnakeBestScoreFormat"), player.HighScore);
    }

    private async void OnLanguageClicked(object? sender, EventArgs e)
    {
        if (sender is VisualElement btn)
        {
            await btn.ScaleToAsync(0.92, 60, Easing.CubicIn);
            await btn.ScaleToAsync(1.0, 60, Easing.CubicOut);
        }

        var code = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "et"
            ? "en-US"
            : "et-EE";

        LanguageService.ChangeLanguage(code);
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
