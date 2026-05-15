using Example.SnakeGame.Services;
using Example.SnakeGame.ViewModels;

namespace Example.SnakeGame;

public partial class SnakeGameStats : ContentPage
{
    private readonly SnakeStatsViewModel _viewModel = new();

    public SnakeGameStats()
    {
        InitializeComponent();
        
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        StatsService.StatsChanged -= RefreshStats;
        StatsService.StatsChanged += RefreshStats;
        
        BindingContext = _viewModel;
        
        UpdateText();
        RefreshStats();
        Theme.Current.Apply(this);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateText();
        RefreshStats();
        Theme.Current.Apply(this);
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

    private async void OnBackClicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();
}
