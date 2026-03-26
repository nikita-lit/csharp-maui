using System.Globalization;

namespace Example.TicTacToe;

public partial class TicTacToeStart : ContentPage
{
    public TicTacToeStart()
    {
        InitializeComponent();
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        string pSym = string.IsNullOrWhiteSpace(PlayerSymbolEntry.Text) ? "X" : PlayerSymbolEntry.Text.Trim();
        string oSym = string.IsNullOrWhiteSpace(OpponentSymbolEntry.Text) ? "O" : OpponentSymbolEntry.Text.Trim();
        
        if (pSym == oSym)
            oSym = pSym == "X" ? "O" : "X";

        int first = FirstPlayerPicker.SelectedIndex;
        bool isBot = OpponentPicker.SelectedIndex == 1;
        int gridSize = 3;

        var players = new List<Player>
        {
            new Player("Mängija 1", pSym, false),
            new Player("Mängija 2", oSym, isBot),
        };

        await Navigation.PushAsync(new TicTacToe(
            new GameData(players, first, gridSize)
        ));
    }

    private void OnSymbolTextChanged(object? sender, EventArgs e)
    {
        if (sender is not Entry entry)
            return;

        var si = new StringInfo(entry.Text);
        if (si.LengthInTextElements > 1)
            entry.Text = si.SubstringByTextElements(0, 1);
    }
}
