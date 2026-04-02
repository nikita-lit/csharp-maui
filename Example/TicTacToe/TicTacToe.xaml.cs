namespace Example.TicTacToe;

public partial class TicTacToe : ContentPage
{
    private readonly GameData _gameData;
    private Player _curPlayer;

    private int _draws = 0;
    private Button[,] _boardButtons;
    private bool _isGameActive = true;
    private bool _isProcessingMove = false;
    private bool _isRoundEnding = false;
    private bool _isStatsLogged = false;
    private bool _isWaitingForReset = false;
    private CancellationTokenSource? _timerCts;

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
        FindFirstPlayer();
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
                var cellContainer = new Grid
                {
                    BackgroundColor = Colors.White,
                    Padding = 0,
                    Margin = 0
                };

                var but = new Button
                {
                    Text = "",
                    FontSize = _gameData.GridSize == 3 ? 36 : (_gameData.GridSize == 4 ? 28 : 22),
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Black,
                    CornerRadius = 0,
                    Margin = 0
                };

                int lX = x;
                int lY = y;
                but.Clicked += (s, e) => OnCellClicked(lX, lY);

                cellContainer.Children.Add(but);
                GridBoard.Add(cellContainer, y, x);
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
            index = Random.Shared.Next(_gameData.Players.Count);
        
        _curPlayer = _gameData.Players[index];
        CurrentPlayerLabel.Text = $"Kord: {_curPlayer.Name} ({_curPlayer.Symbol})";
        CurrentPlayerLabel.Opacity = 1;

        if (_curPlayer.IsBot)
        {
            Dispatcher.Dispatch(async () => {
                await MakeBotMoveAsync();
            });
        }

        StartTurnTimer();
    }

    private async void OnCellClicked(int x, int y)
    {
        if (!_isGameActive || !string.IsNullOrEmpty(_boardButtons[x, y].Text) || _isProcessingMove)
            return;

        if (_curPlayer.IsBot)
            return;

        _isProcessingMove = true;
 
        await MakeMoveAsync(x, y);
        
        if (_isGameActive && _curPlayer.IsBot)
        {
            await MakeBotMoveAsync();
        }
        else if (_isGameActive)
        {
            _isProcessingMove = false;
        }
    }

    private async Task<bool> MakeMoveAsync(int x, int y)
    {
        _timerCts?.Cancel();
        var but = _boardButtons[x, y];
        but.Text = _curPlayer.Symbol;
        but.TextColor = _scoreLabels[_curPlayer].TextColor;

        but.Scale = 0;
        await but.ScaleToAsync(1.2, 150, Easing.CubicOut);
        await but.ScaleToAsync(1.0, 100, Easing.CubicIn);
 
        if (!_isGameActive || _isRoundEnding) 
            return true;
 
        if (CheckWinner())
        {
            _isGameActive = false;
            _isRoundEnding = true;
            UpdateScore(_curPlayer);
            
            CurrentPlayerLabel.Opacity = 0;
            CurrentPlayerLabel.Text = $"Mäng läbi! {_curPlayer.Name} võitis!";
            CurrentPlayerLabel.TextColor = Colors.Green;
            
            CurrentPlayerLabel.Scale = 0.5;
            await Task.WhenAll(
                CurrentPlayerLabel.FadeToAsync(1, 400, Easing.CubicOut),
                CurrentPlayerLabel.ScaleToAsync(1.2, 400, Easing.SpringOut)
            );
 
            if (!_isStatsLogged)
            {
                _isStatsLogged = true;
                StatsManager.AddWin(_curPlayer.Name, _curPlayer.IsBot, _gameData.Players[0].Name);
            }

            _ = HandleRoundEndAsync();
            return true;
        }
        else if (CheckDraw())
        {
            _isGameActive = false;
            _isRoundEnding = true;
            _draws++;
            DrawsLabel.Text = $"Viigid: {_draws}";
            
            CurrentPlayerLabel.Opacity = 0;
            CurrentPlayerLabel.Text = "Mäng läbi! Viik!";
            CurrentPlayerLabel.TextColor = Colors.Orange;
            
            CurrentPlayerLabel.Scale = 0.5;
            await Task.WhenAll(
                CurrentPlayerLabel.FadeToAsync(1, 400, Easing.CubicOut),
                CurrentPlayerLabel.ScaleToAsync(1.2, 400, Easing.SpringOut)
            );
 
            if (!_isStatsLogged)
            {
                _isStatsLogged = true;
                StatsManager.AddDraw();
            }

            _ = HandleRoundEndAsync();
            return true;
        }
        else
        {
            _curPlayer = _gameData.Players[(_gameData.Players.IndexOf(_curPlayer) + 1) % _gameData.Players.Count];
            
            if (!_curPlayer.IsBot)
            {
                await CurrentPlayerLabel.FadeToAsync(0, 150);
                CurrentPlayerLabel.Text = $"Kord: {_curPlayer.Name} ({_curPlayer.Symbol})";
                await CurrentPlayerLabel.FadeToAsync(1, 150);
            }

            StartTurnTimer();
            return false;
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

    private async Task MakeBotMoveAsync()
    {
        if (!_isGameActive) 
            return;

        _isProcessingMove = true;
        
        await CurrentPlayerLabel.FadeToAsync(0, 150);
        CurrentPlayerLabel.Text = $"{_curPlayer.Name} mõtleb...";
        await CurrentPlayerLabel.FadeToAsync(1, 150);

        await Task.Delay(500);

        if (!_isGameActive)
        {
            _isProcessingMove = false;
            return;
        }

        var move = _curPlayer.GetBotMove(_gameData.GridSize, GetBoard());
        
        if (move.HasValue)
        {
            await MakeMoveAsync(move.Value.x, move.Value.y);
        }
 
        if (_isGameActive && !_curPlayer.IsBot)
            _isProcessingMove = false;
    }

    private void DrawWinLine(int type, int index)
    {
        WinLine.IsVisible = true;
        WinLine.Opacity = 0;
        WinLine.Stroke = Colors.Red;

        double cellSize = 300.0 / _gameData.GridSize;
        double halfCell = cellSize / 2;

        if (type == 0) // row
        {
            WinLine.X1 = 0;
            WinLine.X2 = 300;
            WinLine.Y1 = index * cellSize + halfCell;
            WinLine.Y2 = index * cellSize + halfCell;
        }
        else if (type == 1) // col
        {
            WinLine.Y1 = 0;
            WinLine.Y2 = 300;
            WinLine.X1 = index * cellSize + halfCell;
            WinLine.X2 = index * cellSize + halfCell;
        }
        else if (type == 2) // dig 1
        {
            WinLine.X1 = 0;
            WinLine.Y1 = 0;
            WinLine.X2 = 300;
            WinLine.Y2 = 300;
        }
        else if (type == 3) // dig 2
        {
            WinLine.X1 = 300;
            WinLine.Y1 = 0;
            WinLine.X2 = 0;
            WinLine.Y2 = 300;
        }

        WinLine.FadeToAsync(1, 400);
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

            if (rowWin)
            {
                DrawWinLine(0, i);
                return true;
            }
            if (colWin)
            {
                DrawWinLine(1, i);
                return true;
            }
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

        if (diag1Win)
        {
            DrawWinLine(2, 0);
            return true;
        }
        if (diag2Win)
        {
            DrawWinLine(3, 0);
            return true;
        }

        return false;
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

    private void StartTurnTimer()
    {
        _timerCts?.Cancel();
        if (!Config.IsTimerEnabled || !_isGameActive) 
        {
            TimerLabel.IsVisible = false;
            return;
        }

        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        TimerLabel.IsVisible = true;
        TimerLabel.Text = $"Aeg: {(double)Config.TimerSeconds:F1}";

        Task.Run(async () => {
            double remaining = Config.TimerSeconds;
            try 
            {
                while (remaining > 0 && !token.IsCancellationRequested && _isGameActive && !_isRoundEnding)
                {
                    await Task.Delay(100, token);
                    if (token.IsCancellationRequested || !_isGameActive || _isRoundEnding) 
                        break;
                    
                    remaining -= 0.1;
                    
                    if (remaining < 0) 
                        remaining = 0;

                    MainThread.BeginInvokeOnMainThread(() => {
                        TimerLabel.Text = $"Aeg: {remaining:F1}";
                    });
                }
 
                if (remaining <= 0 && !token.IsCancellationRequested && _isGameActive && !_isRoundEnding)
                {
                    MainThread.BeginInvokeOnMainThread(async () => {
                        if (_isGameActive && !_isRoundEnding)
                        {
                            await HandleTimeUp();
                        }
                    });
                }
            } 
            catch (OperationCanceledException) 
            { 

            }
        }, token);
    }

    private async Task HandleTimeUp()
    {
        if (!_isGameActive || _isRoundEnding) 
            return;
            
        _isGameActive = false;
        _isRoundEnding = true;
        _isProcessingMove = true;

        var winner = _gameData.Players.FirstOrDefault(p => p != _curPlayer) ?? _curPlayer;
        UpdateScore(winner);

        CurrentPlayerLabel.Text = $"Aeg läbi! {winner.Name} võitis!";
        CurrentPlayerLabel.TextColor = Colors.Red;
        TimerLabel.Text = "0";
 
        if (!_isStatsLogged)
        {
            _isStatsLogged = true;
            StatsManager.AddWin(winner.Name, winner.IsBot, _gameData.Players[0].Name);
        }
 
        await HandleRoundEndAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isGameActive = false;
        _isRoundEnding = true;
        _timerCts?.Cancel();
        ResetGame();
    }

    private void ResetGame()
    {
        _isGameActive = true;
        _isRoundEnding = false;
        _isStatsLogged = false;
        _isWaitingForReset = false;
        _isProcessingMove = false;

        CurrentPlayerLabel.TextColor = Colors.Black;
        CurrentPlayerLabel.Scale = 1;
        CurrentPlayerLabel.Opacity = 1;

        if (WinLine != null)
            WinLine.IsVisible = false;
        
        if (_boardButtons != null)
        {
            foreach (var but in _boardButtons)
            {
                but.Text = "";
                but.BackgroundColor = Colors.Transparent;
                but.Scale = 1;
            }
        }
    }

    private async Task HandleRoundEndAsync()
    {
        if (_isWaitingForReset) 
            return;
            
        _isWaitingForReset = true;

        await Task.Delay(1500);
        if (!IsVisible) 
            return;

        ResetGame();
        FindFirstPlayer();
    }

    private async void OnEndGameClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}