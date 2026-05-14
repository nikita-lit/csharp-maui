using Example.SnakeGame.Services;
using Example.SnakeGame.ViewModels;

namespace Example.SnakeGame;

public partial class SnakeGamePage : ContentPage
{
    private readonly SnakeGameViewModel _viewModel = new();
    private Game _game;
    private Player _player;
    private BoxView[,] _cells = null!;
    private readonly List<Cell> _paintedCells = [];
    private const int GridSize = 15;
    private Theme _theme = Theme.Current;

    public SnakeGamePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;

        _player = SnakeGameViewModel.CreatePlayer();
        _game = new Game(_player, GridSize, SnakeGameViewModel.Speed);

        BuildGrid();
        ApplyTheme(Theme.Current);
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
        var cellSize = boardSize / GridSize;

        for (var i = 0; i < GridSize; i++)
        {
            GameBoard.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            GameBoard.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(cellSize)));
        }

        _cells = new BoxView[GridSize, GridSize];

        for (var r = 0; r < GridSize; r++)
        {
            for (var c = 0; c < GridSize; c++)
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

    private void ApplyTheme(Theme theme)
    {
        _theme = theme;
        
        BackgroundColor = _theme.BackgroundColor;
        RootGrid.BackgroundColor = _theme.BackgroundColor;
        ScoreLabel.TextColor = _theme.TextColor;
        HighScoreLabel.TextColor = _theme.TextColor;

        foreach (var btn in new[] { UpBtn, DownBtn, LeftBtn, RightBtn })
            StyleButton(btn, _theme.GridColor, _theme.TextColor);

        StyleButton(PauseButton, _theme.ButtonColor, _theme.ButtonTextColor);
        StyleButton(RestartButton, _theme.GridColor, _theme.TextColor);

        ResetBoard();
    }

    private static void StyleButton(Button button, Color background, Color text)
    {
        button.BackgroundColor = background;
        button.TextColor = text;
    }

    private void StartNewGame()
    {
        _game.Stop();
        _player = SnakeGameViewModel.CreatePlayer();
        _game = new Game(_player, GridSize, SnakeGameViewModel.Speed);
        _game.OnUpdate += OnGameUpdate;
        _game.OnGameOver += OnGameOver;
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
        foreach (var cell in _paintedCells.Where(IsInside))
            _cells[cell.Row, cell.Col].Color = _theme.GridColor;

        _paintedCells.Clear();

        for (var i = 1; i < _game.Snake.Body.Count; i++)
        {
            var part = _game.Snake.Body[i];
            PaintCell(part, _theme.SnakeColor);
        }

        PaintCell(_game.Snake.Head, _theme.SnakeHeadColor);
        PaintCell(_game.Food.Position, _theme.FoodColor);
    }

    private void PaintCell(Cell cell, Color color)
    {
        if (!IsInside(cell))
            return;

        _cells[cell.Row, cell.Col].Color = color;
        _paintedCells.Add(cell);
    }

    private void ResetBoard()
    {
        _paintedCells.Clear();
        for (var r = 0; r < GridSize; r++)
            for (var c = 0; c < GridSize; c++)
                _cells[r, c].Color = _theme.GridColor;
    }

    private static bool IsInside(Cell cell) =>
        cell.Row is >= 0 and < GridSize && cell.Col is >= 0 and < GridSize;

    private void UpdateScore()
    {
        _viewModel.UpdateScore(_player);
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeGameTitle");
        _viewModel.UpdateScore(_player);
    }

    private async void OnGameOver()
    {
        for (var r = 0; r < GridSize; r++)
            for (var c = 0; c < GridSize; c++)
                _cells[r, c].Color = _theme.FoodColor;

        await Task.Delay(300);
        ResetBoard();
        RenderBoard();
        StatsService.SaveResult(_player);

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
        => SetDirection(Snake.Up);

    private void OnDownClicked(object? sender, EventArgs e) 
        => SetDirection(Snake.Down);

    private void OnLeftClicked(object? sender, EventArgs e) 
        => SetDirection(Snake.Left);

    private void OnRightClicked(object? sender, EventArgs e) 
        => SetDirection(Snake.Right);

    public void SetDirection(int direction) => _game.SetDirection(direction);

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
