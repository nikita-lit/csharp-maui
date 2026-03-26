namespace Example.TicTacToe;

public partial class TicTacToe : ContentPage
{
    private List<Player> _players;
    private Player _currentPlayer;
    private int _currentPlayerIndex;
    
    private int _firstPlayerOpt;
    private int _gridSize;

    private int _draws = 0;
    private Button[,] _boardButtons;
    private bool _isGameActive = true;

    private Dictionary<Player, Label> _scoreLabels;
    private Label _drawsLabel;
    private Label _currentPlayerLabel;
    private Grid _gameBoard;

    public TicTacToe(List<Player> players, int firstPlayerOpt, int gridSize)
    {
        InitializeComponent();
        
        _players = players;
        _firstPlayerOpt = firstPlayerOpt;
        _gridSize = gridSize;

        var mainLayout = new VerticalStackLayout { 
            Padding = 20, 
            Spacing = 15 
        };

        mainLayout.Children.Add(new Label { 
            Text = "Trips-Traps-Trull", 
            FontSize = 32, 
            FontAttributes = FontAttributes.Bold, 
            HorizontalOptions = LayoutOptions.Center 
        });

        var statsLayout = new HorizontalStackLayout { 
            Spacing = 20, 
            HorizontalOptions = LayoutOptions.Center 
        };
        
        _scoreLabels = new Dictionary<Player, Label>();
        for (int i = 0; i < _players.Count; i++)
        {
            var p = _players[i];
            var lbl = new Label { 
                Text = $"{p.Name} ({p.Symbol}): 0", 
                FontSize = 18, 
                FontAttributes = FontAttributes.Bold, 
                TextColor = Colors.Black
            };
            _scoreLabels[p] = lbl;
            statsLayout.Children.Add(lbl);
        }

        _drawsLabel = new Label { 
            Text = "Viigid: 0", 
            FontSize = 18, 
            FontAttributes = FontAttributes.Bold, 
            TextColor = Colors.Gray 
        };
        statsLayout.Children.Add(_drawsLabel);
        mainLayout.Children.Add(statsLayout);

        _currentPlayerLabel = new Label { 
            Text = "Kord: ", 
            FontSize = 20, 
            HorizontalOptions = LayoutOptions.Center, 
            Margin = new Thickness(0, 10, 0, 0), 
            TextColor = Colors.Black
        };
        mainLayout.Children.Add(_currentPlayerLabel);

        _gameBoard = new Grid { 
            WidthRequest = 300, 
            HeightRequest = 300, 
            BackgroundColor = Colors.LightGray, 
            ColumnSpacing = 2, 
            RowSpacing = 2 
        };

        var boardFrame = new Frame { 
            BorderColor = Colors.Gray, 
            Padding = 0,
            HasShadow = true, 
            HorizontalOptions = LayoutOptions.Center, 
            VerticalOptions = LayoutOptions.Center, 
            Content = _gameBoard 
        };
        mainLayout.Children.Add(boardFrame);

        BackgroundColor = Colors.White;
        Content = new ScrollView { 
            Content = mainLayout 
        };

        InitBoard();
    }

    private void InitBoard()
    {
        _gameBoard.RowDefinitions.Clear();
        _gameBoard.ColumnDefinitions.Clear();
        _gameBoard.Children.Clear();

        for (int i = 0; i < _gridSize; i++)
        {
            _gameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            _gameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        _boardButtons = new Button[_gridSize, _gridSize];

        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
            {
                var but = new Button
                {
                    Text = "",
                    FontSize = _gridSize == 3 ? 36 : (_gridSize == 4 ? 28 : 22),
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Colors.White,
                    TextColor = Colors.Black,
                    CornerRadius = 0,
                    Margin = 0
                };

                int r = row;
                int c = col;
                but.Clicked += (s, e) => OnCellClicked(r, c);

                _gameBoard.Add(but, col, row);
                _boardButtons[row, col] = but;
            }
        }

        ResetGame();
    }

    private void DetermineFirstPlayer()
    {
        if (_firstPlayerOpt >= 0 && _firstPlayerOpt < _players.Count)
            _currentPlayerIndex = _firstPlayerOpt;
        else
            _currentPlayerIndex = new Random().Next(_players.Count);
        
        _currentPlayer = _players[_currentPlayerIndex];
        _currentPlayerLabel.Text = $"Kord: {_currentPlayer.Symbol}";

        if (_currentPlayer.IsBot)
            MakeBotMove();
    }

    private void OnCellClicked(int row, int col)
    {
        if (!_isGameActive || !string.IsNullOrEmpty(_boardButtons[row, col].Text))
            return;

        if (_currentPlayer.IsBot)
            return;

        MakeMove(row, col);

        if (_isGameActive && _currentPlayer.IsBot)
            MakeBotMove();
    }

    private async void MakeMove(int row, int col)
    {
        var but = _boardButtons[row, col];
        but.Text = _currentPlayer.Symbol;
        but.TextColor = _scoreLabels[_currentPlayer].TextColor;

        if (CheckWinner())
        {
            _isGameActive = false;
            UpdateScore(_currentPlayer);
            
            bool p = await DisplayAlertAsync("Mäng läbi", $"{_currentPlayer.Name} võitis! Kas soovid veel mängida?", "Jah", "Ei");
            if (p) 
                ResetGame();
        }
        else if (CheckDraw())
        {
            _isGameActive = false;
            _draws++;
            _drawsLabel.Text = $"Viigid: {_draws}";
            
            bool p = await DisplayAlertAsync("Mäng läbi", "Viik! Kas soovid veel mängida?", "Jah", "Ei");
            if (p) 
                ResetGame();
        }
        else
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _currentPlayer = _players[_currentPlayerIndex];
            _currentPlayerLabel.Text = $"Kord: {_currentPlayer.Symbol}";
        }
    }

    private void MakeBotMove()
    {
        var emptyCells = new List<(int r, int c)>();
        for (int r = 0; r < _gridSize; r++)
        {
            for (int c = 0; c < _gridSize; c++)
            {
                if (string.IsNullOrEmpty(_boardButtons[r, c].Text))
                    emptyCells.Add((r, c));
            }
        }

        if (emptyCells.Count > 0)
        {
            var random = new Random();
            var cell = emptyCells[random.Next(emptyCells.Count)];
            MakeMove(cell.r, cell.c);
        }
    }

    private bool CheckWinner()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            bool rowWin = true;
            bool colWin = true;
            for (int j = 0; j < _gridSize; j++)
            {
                if (_boardButtons[i, j].Text != _currentPlayer.Symbol) 
                    rowWin = false;

                if (_boardButtons[j, i].Text != _currentPlayer.Symbol) 
                    colWin = false;
            }

            if (rowWin || colWin) 
                return true;
        }

        bool diag1Win = true;
        bool diag2Win = true;
        for (int i = 0; i < _gridSize; i++)
        {
            if (_boardButtons[i, i].Text != _currentPlayer.Symbol) 
                diag1Win = false;

            if (_boardButtons[i, _gridSize - 1 - i].Text != _currentPlayer.Symbol) 
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

        DetermineFirstPlayer();
    }
}