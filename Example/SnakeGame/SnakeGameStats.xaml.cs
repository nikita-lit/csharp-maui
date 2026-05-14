using Example.SnakeGame.Services;
using Example.SnakeGame.ViewModels;

namespace Example.SnakeGame;

public partial class SnakeGameStats : ContentPage
{
    private readonly SnakeStatsViewModel _viewModel = new();

    public SnakeGameStats()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        UpdateText();
        ApplyTheme(Theme.Current);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        StatsService.StatsChanged -= RefreshStats;
        StatsService.StatsChanged += RefreshStats;

        RefreshStats();
        ApplyTheme(Theme.Current);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
        StatsService.StatsChanged -= RefreshStats;
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
        BackgroundColor = theme.BackgroundColor;
        MainLayout.BackgroundColor = theme.BackgroundColor;
        StatsPanel.BackgroundColor = theme.GridColor;
        StatsTitleLabel.TextColor = theme.TextColor;
        PlayerNameLabel.TextColor = theme.TextColor;
        GamesPlayedLabel.TextColor = theme.TextColor;
        LastScoreLabel.TextColor = theme.TextColor;
        BestScoreLabel.TextColor = theme.TextColor;
    }
}
