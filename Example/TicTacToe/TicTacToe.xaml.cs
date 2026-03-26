namespace Example.TicTacToe;

public partial class TicTacToe : ContentPage
{
    private readonly GameData _gameData;
    private Player _curPlayer;

    private int _draws = 0;
    private Button[,] _boardButtons;
    private bool _isGameActive = true;

    private Dictionary<Player, Label> _scoreLabels;
    private Label _drawsLabel;
    private Label _currentPlayerLabel;
    private Grid _gameBoard;

    public TicTacToe(GameData data)
    {
        _gameData = data;

        InitializeComponent();
        
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

        for (int i = 0; i < _gameData.GridSize; i++)
        {
            _gameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            _gameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        _boardButtons = new Button[_gameData.GridSize, _gameData.GridSize];

        for (int row = 0; row < _gameData.GridSize; row++)
        {
            for (int col = 0; col < _gameData.GridSize; col++)
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
        int index = 0;
        if (_gameData.FirstPlayer >= 0 && _gameData.FirstPlayer < _gameData.Players.Count)
            index = _gameData.FirstPlayer;
        else
            index = new Random().Next(_gameData.Players.Count);
        
        _curPlayer = _gameData.Players[index];
        _currentPlayerLabel.Text = $"Kord: {_curPlayer.Symbol}";

        if (_curPlayer.IsBot)
            MakeBotMove();
    }

    private void OnCellClicked(int row, int col)
    {
        if (!_isGameActive || !string.IsNullOrEmpty(_boardButtons[row, col].Text))
            return;

        if (_curPlayer.IsBot)
            return;

        MakeMove(row, col);

        if (_isGameActive && _curPlayer.IsBot)
            MakeBotMove();
    }

    private async void MakeMove(int row, int col)
    {
        var but = _boardButtons[row, col];
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
            _drawsLabel.Text = $"Viigid: {_draws}";
            
            bool p = await DisplayAlertAsync("Mäng läbi", "Viik! Kas soovid veel mängida?", "Jah", "Ei");
            if (p) 
                ResetGame();
            else
                await Navigation.PopAsync();
        }
        else
        {
            _curPlayer = _gameData.Players[(_gameData.Players.IndexOf(_curPlayer) + 1) % _gameData.Players.Count];
            _currentPlayerLabel.Text = $"Kord: {_curPlayer.Symbol}";
        }
    }

    private void MakeBotMove()
    {
        var emptyCells = new List<(int r, int c)>();
        for (int r = 0; r < _gameData.GridSize; r++)
        {
            for (int c = 0; c < _gameData.GridSize; c++)
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

        DetermineFirstPlayer();
    }
}