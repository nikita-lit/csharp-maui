namespace Example.SnakeGame;

public partial class SnakeGameStats : ContentPage
{
    private readonly SnakeStatsViewModel _viewModel = new();
    private Theme _theme;

    public SnakeGameStats()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        _theme = ThemeService.CurrentTheme;
        UpdateText();
        ApplyTheme(_theme);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.ThemeChanged -= OnThemeChanged;
        ThemeService.ThemeChanged += OnThemeChanged;
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        SnakeStatsService.StatsChanged -= RefreshStats;
        SnakeStatsService.StatsChanged += RefreshStats;

        RefreshStats();
        ApplyTheme(ThemeService.CurrentTheme);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeService.ThemeChanged -= OnThemeChanged;
        LanguageService.LanguageChanged -= UpdateText;
        SnakeStatsService.StatsChanged -= RefreshStats;
    }

    private void OnThemeChanged(Theme theme)
    {
        _theme = theme;
        ApplyTheme(_theme);
    }

    private void RefreshStats()
    {
        _viewModel.Refresh();
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeStatsTitle");
        StatsTitleLabel.Text = LanguageService.Get("SnakeStats");
        RefreshStats();
    }

    private void ApplyTheme(Theme theme)
    {
        theme.Apply(this);
        MainLayout.BackgroundColor = theme.BackgroundColor;
        StatsPanel.BackgroundColor = theme.GridColor;
        StatsTitleLabel.TextColor = theme.TextColor;
        PlayerNameLabel.TextColor = theme.TextColor;
        GamesPlayedLabel.TextColor = theme.TextColor;
        LastScoreLabel.TextColor = theme.TextColor;
        BestScoreLabel.TextColor = theme.TextColor;
    }
}
