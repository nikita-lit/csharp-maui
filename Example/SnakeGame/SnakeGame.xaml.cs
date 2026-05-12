namespace Example.SnakeGame;

public partial class SnakeGamePage : ContentPage
{
    private Game _game;
    private Player _player;
    private Theme _theme;
    private BoxView[,] _cells = null!;
    private int _gridSize = 15;
    private int _speed;

    public SnakeGamePage()
    {
        InitializeComponent();

        _theme = Theme.LoadSelected();
        _speed = Preferences.Get("snake_speed", 200);
        var playerName = Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));
        _player = new Player(playerName);

        _game = new Game(_player, _theme, _gridSize, _speed);

        BuildGrid();
        ApplyTheme();
        UpdateText();

        AddSwipe(SwipeDirection.Up, Snake.Up);
        AddSwipe(SwipeDirection.Down, Snake.Down);
        AddSwipe(SwipeDirection.Left, Snake.Left);
        AddSwipe(SwipeDirection.Right, Snake.Right);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        UpdateText();
        StartNewGame();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
        _game.Stop();
    }

    private void AddSwipe(SwipeDirection swipeDirection, int snakeDirection)
    {
        var swipe = new SwipeGestureRecognizer { Direction = swipeDirection };
        swipe.Swiped += (s, e) => _game.SetDirection(snakeDirection);
        GameBoard.GestureRecognizers.Add(swipe);
    }

    private void BuildGrid()
    {
        GameBoard.Children.Clear();
        GameBoard.RowDefinitions.Clear();
        GameBoard.ColumnDefinitions.Clear();

        var boardSize = Math.Min(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - 40, 400);
        var cellSize = boardSize / _gridSize;

        for (var i = 0; i < _gridSize; i++)
        {
            GameBoard.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            GameBoard.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(cellSize)));
        }

        _cells = new BoxView[_gridSize, _gridSize];

        for (var r = 0; r < _gridSize; r++)
        {
            for (var c = 0; c < _gridSize; c++)
            {
                var box = new BoxView
                {
                    CornerRadius = 2,
                    Margin = 0.5
                };
                
                Grid.SetRow(box, r);
                Grid.SetColumn(box, c);
                GameBoard.Children.Add(box);
                _cells[r, c] = box;
            }
        }
    }

    private void ApplyTheme()
    {
        _theme.Apply(this);
        RootGrid.BackgroundColor = _theme.BackgroundColor;
        ScoreLabel.TextColor = _theme.TextColor;
        HighScoreLabel.TextColor = _theme.TextColor;

        foreach (var btn in new[] { UpBtn, DownBtn, LeftBtn, RightBtn })
            StyleButton(btn, _theme.GridColor, _theme.TextColor);

        StyleButton(PauseButton, _theme.ButtonColor, _theme.ButtonTextColor);
        StyleButton(RestartButton, _theme.GridColor, _theme.TextColor);

        RenderBoard();
    }

    private static void StyleButton(Button button, Color background, Color text)
    {
        button.BackgroundColor = background;
        button.TextColor = text;
    }

    private void StartNewGame()
    {
        _player.ResetScore();
        _game = new Game(_player, _theme, _gridSize, _speed);
        _game.OnUpdate += OnGameUpdate;
        _game.OnGameOver += OnGameOverHandler;
        _game.Start(Dispatcher);

        UpdateScore();
        RenderBoard();
    }

    private void OnGameUpdate()
    {
        RenderBoard();
        UpdateScore();
    }

    private void RenderBoard()
    {
        for (var r = 0; r < _gridSize; r++)
        {
            for (var c = 0; c < _gridSize; c++)
                _cells[r, c].Color = _theme.GridColor;
        }

        for (var i = 1; i < _game.Snake.Body.Count; i++)
        {
            var part = _game.Snake.Body[i];
            if (IsInside(part))
                _cells[part.Row, part.Col].Color = _theme.SnakeColor;
        }

        var head = _game.Snake.Head;
        if (IsInside(head))
            _cells[head.Row, head.Col].Color = _theme.SnakeHeadColor;

        var food = _game.Food.Position;
        if (IsInside(food))
            _cells[food.Row, food.Col].Color = _theme.FoodColor;
    }

    private bool IsInside(Cell cell) =>
        cell.Row >= 0 && cell.Row < _gridSize && cell.Col >= 0 && cell.Col < _gridSize;

    private void UpdateScore()
    {
        ScoreLabel.Text = string.Format(LanguageService.Get("SnakeScoreFormat"), _player.Score);
        HighScoreLabel.Text = string.Format(LanguageService.Get("SnakeHighScoreFormat"), _player.HighScore);
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeGameTitle");
        UpdateScore();
    }

    private async void OnGameOverHandler()
    {
        for (var r = 0; r < _gridSize; r++)
            for (var c = 0; c < _gridSize; c++)
                _cells[r, c].Color = _theme.FoodColor;

        await Task.Delay(300);
        RenderBoard();

        var again = await DisplayAlertAsync(
            LanguageService.Get("SnakeGameOverTitle"),
            string.Format(
                LanguageService.Get("SnakeGameOverMessageFormat"),
                _player.Name,
                _player.Score,
                _player.HighScore),
            LanguageService.Get("SnakePlayAgain"),
            LanguageService.Get("SnakeBack")
        );

        if (again)
            StartNewGame();
        else
            await Navigation.PopAsync();
    }

    private void OnUpClicked(object? sender, EventArgs e) 
        => _game.SetDirection(Snake.Up);

    private void OnDownClicked(object? sender, EventArgs e) 
        => _game.SetDirection(Snake.Down);

    private void OnLeftClicked(object? sender, EventArgs e) 
        => _game.SetDirection(Snake.Left);

    private void OnRightClicked(object? sender, EventArgs e) 
        => _game.SetDirection(Snake.Right);

    private void OnPauseClicked(object? sender, EventArgs e)
    {
        _game.TogglePause();
        PauseButton.Text = _game.IsRunning ? "⏸" : "▶";
    }

    private void OnRestartClicked(object? sender, EventArgs e)
    {
        _game.Stop();
        StartNewGame();
    }
}
