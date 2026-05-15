using Example.SnakeGame.Services;
using Example.SnakeGame.ViewModels;
using Example.SnakeGame.Models;
using Cell = Example.SnakeGame.Models.Cell;

namespace Example.SnakeGame;

public partial class SnakeGamePage : ContentPage
{
    private readonly SnakeGameViewModel _viewModel = new();
    private Game _game;
    private Player _player;
    private Cell[,] _cells = null!;
    private const int GridSize = 15;

    public SnakeGamePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;

        _player = SnakeGameViewModel.CreatePlayer();
        _game = new Game(_player, GridSize, SnakeGameViewModel.Speed);

        BuildGrid();
        
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
        
        Theme.Current.Apply(this);
        ApplyTheme(Theme.Current);
        UpdateText();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartNewGame();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
        _game.Stop();
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

        _cells = new Cell[GridSize, GridSize];

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
                _cells[r, c] = new Cell(r, c, box);
            }
        }
    }

    private void ApplyTheme(Theme theme)
    {
        RootGrid.BackgroundColor = theme.BackgroundColor;

        foreach (var btn in new[] { UpBtn, DownBtn, LeftBtn, RightBtn })
            btn.BackgroundColor = theme.GridColor;

        PauseButton.BackgroundColor = theme.ButtonColor;
        RestartButton.BackgroundColor = theme.GridColor;

        ClearBoard();
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
    
    private void PaintCell((int Row, int Col) pos, Color color)
    {
        _cells[pos.Row, pos.Col].SetColor(color);
    }

    private void RenderBoard()
    {
        var theme = Theme.Current;
        
        ClearBoard();
        
        for (var i = 1; i < _game.Snake.Body.Count; i++)
        {
            var part = _game.Snake.Body[i];
            PaintCell(part, theme.SnakeColor);
        }

        PaintCell(_game.Snake.Head, theme.SnakeHeadColor);
        
        foreach (var food in _game.Foods)
        {
            var foodColor = food.Type switch
            {
                FoodType.Gold => theme.GoldColor,
                FoodType.Poison => theme.PoisonColor,
                _ => theme.FoodColor
            };

            PaintCell(food.Position, foodColor);
        }
    }

    private void ClearBoard()
    {
        var theme = Theme.Current;
        
        for (var r = 0; r < GridSize; r++)
            for (var c = 0; c < GridSize; c++)
                PaintCell((r, c), theme.GridColor);
    }
    
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
        var theme = Theme.Current;
        
        for (var r = 0; r < GridSize; r++)
            for (var c = 0; c < GridSize; c++)
                PaintCell((r, c), theme.FoodColor);

        await Task.Delay(300);
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

    public void SetDirection(int direction) 
        => _game.SetDirection(direction);

    private void OnPauseClicked(object? sender, EventArgs e)
    {
        _game.TogglePause();
        PauseButton.Source = _game.IsRunning ? "pause.png" : "play.png";
    }

    private void OnRestartClicked(object? sender, EventArgs e)
    {
        _game.Stop();
        StartNewGame();
    }
}
