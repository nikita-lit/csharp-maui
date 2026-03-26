namespace Example.TicTacToe;

public partial class TicTacToe : ContentPage
{
    private static readonly Random _random = new();

    private readonly GameData _gameData;
    private Player _curPlayer;

    private int _draws = 0;
    private Button[,] _boardButtons;
    private bool _isGameActive = true;

    private Dictionary<Player, Label> _scoreLabels;

    public TicTacToe(GameData data)
    {
        _gameData = data;

        InitializeComponent();
        
        _scoreLabels = new Dictionary<Player, Label>();
        for (int i = 0; i < _gameData.Players.Count; i++)
        {
            var p = _gameData.Players[i];
            var lbl = new Label { 
                Text = $"{p.Name} ({p.Symbol}): 0", 
                FontSize = 18, 
                FontAttributes = FontAttributes.Bold, 
                TextColor = Colors.Black
            };
            _scoreLabels[p] = lbl;
            StatsLayout.Children.Insert(i, lbl);
        }

        InitBoard();
        ResetGame();
    }

    private void InitBoard()
    {
        GridBoard.RowDefinitions.Clear();
        GridBoard.ColumnDefinitions.Clear();
        GridBoard.Children.Clear();

        for (int i = 0; i < _gameData.GridSize; i++)
        {
            GridBoard.RowDefinitions.Add(new RowDefinition { 
                Height = GridLength.Star 
            });
            GridBoard.ColumnDefinitions.Add(new ColumnDefinition { 
                Width = GridLength.Star 
            });
        }

        _boardButtons = new Button[_gameData.GridSize, _gameData.GridSize];

        for (int x = 0; x < _gameData.GridSize; x++)
        {
            for (int y = 0; y < _gameData.GridSize; y++)
            {
                var but = new Button
                {
                    Text = "",
                    FontSize = _gameData.GridSize == 3 ? 36 : (_gameData.GridSize == 4 ? 28 : 22),
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Colors.White,
                    TextColor = Colors.Black,
                    CornerRadius = 0,
                    Margin = 0
                };

                int lX = x;
                int lY = y;
                but.Clicked += (s, e) => OnCellClicked(lX, lY);

                GridBoard.Add(but, y, x);
                _boardButtons[x, y] = but;
            }
        }
    }

    private void FindFirstPlayer()
    {
        int index;
        if (_gameData.FirstPlayer >= 0 && _gameData.FirstPlayer < _gameData.Players.Count)
            index = _gameData.FirstPlayer;
        else
            index = _random.Next(_gameData.Players.Count);
        
        _curPlayer = _gameData.Players[index];
        CurrentPlayerLabel.Text = $"Kord: {_curPlayer.Symbol}";

        if (_curPlayer.IsBot)
            MakeBotMove();
    }

    private void OnCellClicked(int x, int y)
    {
        if (!_isGameActive || !string.IsNullOrEmpty(_boardButtons[x, y].Text))
            return;

        if (_curPlayer.IsBot)
            return;

        MakeMove(x, y);

        if (_isGameActive && _curPlayer.IsBot)
            MakeBotMove();
    }

    private async void MakeMove(int x, int y)
    {
        var but = _boardButtons[x, y];
        but.Text = _curPlayer.Symbol;
        but.TextColor = _scoreLabels[_curPlayer].TextColor;

        if (CheckWinner())
        {
            _isGameActive = false;
            UpdateScore(_curPlayer);
            
            bool p = await DisplayAlertAsync("Mäng läbi", $"{_curPlayer.Name} võitis! Kas soovid veel mängida?", "Jah", "Ei");
            if (p) 
                ResetGame();
            else
                await Navigation.PopAsync();
        }
        else if (CheckDraw())
        {
            _isGameActive = false;
            _draws++;
            DrawsLabel.Text = $"Viigid: {_draws}";
            
            bool p = await DisplayAlertAsync("Mäng läbi", "Viik! Kas soovid veel mängida?", "Jah", "Ei");
            if (p) 
                ResetGame();
            else
                await Navigation.PopAsync();
        }
        else
        {
            _curPlayer = _gameData.Players[(_gameData.Players.IndexOf(_curPlayer) + 1) % _gameData.Players.Count];
            CurrentPlayerLabel.Text = $"Kord: {_curPlayer.Symbol}";
        }
    }

    private string[,] GetBoard()
    {
        string[,] board = new string[_gameData.GridSize, _gameData.GridSize];
        for (int r = 0; r < _gameData.GridSize; r++)
            for (int c = 0; c < _gameData.GridSize; c++)
                board[r, c] = _boardButtons[r, c].Text;

        return board;
    }

    private void MakeBotMove()
    {
        var move = _curPlayer.GetBotMove(_gameData.GridSize, GetBoard());
        if (move.HasValue)
            MakeMove(move.Value.x, move.Value.y);
    }

    private bool CheckWinner()
    {
        for (int i = 0; i < _gameData.GridSize; i++)
        {
            bool rowWin = true;
            bool colWin = true;
            for (int j = 0; j < _gameData.GridSize; j++)
            {
                if (_boardButtons[i, j].Text != _curPlayer.Symbol) 
                    rowWin = false;

                if (_boardButtons[j, i].Text != _curPlayer.Symbol) 
                    colWin = false;
            }

            if (rowWin || colWin) 
                return true;
        }

        bool diag1Win = true;
        bool diag2Win = true;
        for (int i = 0; i < _gameData.GridSize; i++)
        {
            if (_boardButtons[i, i].Text != _curPlayer.Symbol) 
                diag1Win = false;

            if (_boardButtons[i, _gameData.GridSize - 1 - i].Text != _curPlayer.Symbol) 
                diag2Win = false;
        }

        return diag1Win || diag2Win;
    }

    private bool CheckDraw()
    {
        foreach (var but in _boardButtons)
        {
            if (string.IsNullOrEmpty(but.Text)) 
                return false;
        }

        return true;
    }

    private void UpdateScore(Player winner)
    {
        winner.Points++;
        _scoreLabels[winner].Text = $"{winner.Name} ({winner.Symbol}): {winner.Points}";
    }

    private void ResetGame()
    {
        _isGameActive = true;
        if (_boardButtons != null)
        {
            foreach (var but in _boardButtons)
            {
                but.Text = "";
                but.BackgroundColor = Colors.White;
            }
        }

        FindFirstPlayer();
    }
}