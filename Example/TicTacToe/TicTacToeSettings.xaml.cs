using System.Globalization;

namespace Example.TicTacToe;

public partial class TicTacToeSettings : ContentPage
{
    public TicTacToeSettings()
    {
        InitializeComponent();
        
        Player1NameEntry.Text = Config.Player1Name;
        PlayerSymbolEntry.Text = Config.Player1Symbol;
        Player2NameEntry.Text = Config.Player2Name;
        OpponentSymbolEntry.Text = Config.Player2Symbol;
        
        if (Config.FirstPlayerIndex >= 0 && Config.FirstPlayerIndex < FirstPlayerPicker.Items.Count)
        {
            FirstPlayerPicker.SelectedIndex = Config.FirstPlayerIndex;
        }

        Player1NameEntry.TextChanged += (s, e) => Config.Player1Name = e.NewTextValue;
        Player2NameEntry.TextChanged += (s, e) => Config.Player2Name = e.NewTextValue;

        TimerSwitch.IsToggled = Config.IsTimerEnabled;
        TimerSlider.Value = Config.TimerSeconds;
        TimerLabel.Text = $"Sekundid: {Config.TimerSeconds}";
    }

    private void OnFirstPlayerChanged(object? sender, EventArgs e)
    {
        Config.FirstPlayerIndex = FirstPlayerPicker.SelectedIndex;
    }

    private void OnSymbolTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry || entry.Text == null)
            return;

        var si = new StringInfo(entry.Text);
        if (si.LengthInTextElements > 1)
        {
            entry.Text = si.SubstringByTextElements(0, 1);
        }

        if (entry == PlayerSymbolEntry)
            Config.Player1Symbol = entry.Text;
        else if (entry == OpponentSymbolEntry)
            Config.Player2Symbol = entry.Text;
    }

    private void OnTimerToggled(object? sender, ToggledEventArgs e)
    {
        Config.IsTimerEnabled = e.Value;
    }

    private void OnTimerSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        TimerLabel.Text = $"Sekundid: {(int)e.NewValue}";
    }

    private void OnTimerSliderChanged(object? sender, EventArgs e)
    {
        Config.TimerSeconds = (int)TimerSlider.Value;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
