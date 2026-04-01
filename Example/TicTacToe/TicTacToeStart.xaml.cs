namespace Example.TicTacToe;

public partial class TicTacToeStart : ContentPage
{
    public TicTacToeStart()
    {
        InitializeComponent();
    }

    private async void OnPlayClicked(object? sender, EventArgs e)
    {
        string action = await DisplayActionSheetAsync("Kelle vastu soovid mängida?", "Tühista", null, "Inimese vastu", "Boti vastu");

        if (action == "Tühista" || string.IsNullOrEmpty(action))
            return;

        bool isBot = action == "Boti vastu";

        string p1Name = string.IsNullOrWhiteSpace(Config.Player1Name) ? "Mängija 1" : Config.Player1Name.Trim();
        string p2Name = isBot ? "Bot" : (string.IsNullOrWhiteSpace(Config.Player2Name) ? "Mängija 2" : Config.Player2Name.Trim());

        string pSym = string.IsNullOrWhiteSpace(Config.Player1Symbol) ? "X" : Config.Player1Symbol.Trim();
        string oSym = string.IsNullOrWhiteSpace(Config.Player2Symbol) ? "O" : Config.Player2Symbol.Trim();
        
        if (pSym == oSym)
            oSym = pSym == "X" ? "O" : "X";

        var players = new List<Player>
        {
            new Player(p1Name, pSym, false),
            new Player(p2Name, oSym, isBot)
        };

        await Navigation.PushAsync(new TicTacToe(
            new GameData(players, Config.FirstPlayerIndex, Config.GridSize)
        ));
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new TicTacToeSettings());
    }

    private async void OnStatsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new TicTacToeStats());
    }
}
