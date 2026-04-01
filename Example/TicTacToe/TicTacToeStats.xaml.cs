using System.Collections.ObjectModel;

namespace Example.TicTacToe;

public partial class TicTacToeStats : ContentPage
{
    private GameStats _stats;
    public TicTacToeStats()
    {
        InitializeComponent();
        LoadAndDisplayStats();
    }

    private void LoadAndDisplayStats()
    {
        _stats = StatsManager.LoadStats();
        P1WinsLabel.Text = _stats.Player1Wins.ToString();
        P2WinsLabel.Text = _stats.Player2Wins.ToString();
        BotWinsLabel.Text = _stats.BotWins.ToString();
        DrawsLabel.Text = _stats.Draws.ToString();
        HistoryList.ItemsSource = _stats.History;
    }

    private async void OnResetClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Kinnita", "Kas oled kindel, et soovid kogu ajaloo kustutada?", "Jah", "Ei");
        if (confirm)
        {
            StatsManager.SaveStats(new GameStats());
            LoadAndDisplayStats();
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
