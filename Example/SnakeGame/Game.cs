namespace Example.SnakeGame.Models;

public class Game
{
    private IDispatcherTimer? _timer;

    public event Action? OnUpdate;
    public event Action? OnGameOver;

    public Snake Snake { get; }
    public List<Food> Foods { get; } = new();
    public Player Player { get; }
    public int GridSize { get; }
    public int Speed { get; }
    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }

    private readonly Random _random = new();

    public Game(Player player, int gridSize = 15, int speed = 200)
    {
        Player = player;
        GridSize = gridSize;
        Speed = speed;
        Snake = new Snake(gridSize / 2, gridSize / 2);
        Foods.Add(new Food(gridSize, FoodType.Default, Snake));
    }

    public void Start(IDispatcher dispatcher)
    {
        Player.ResetScore();
        IsRunning = true;
        IsGameOver = false;

        _timer = dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(Speed);
        _timer.Tick += (s, e) => Update();
        _timer.Start();
    }

    public void TogglePause()
    {
        if (IsGameOver)
            return;

        IsRunning = !IsRunning;
        if (IsRunning)
            _timer?.Start();
        else
            _timer?.Stop();
    }

    public void Stop()
    {
        IsRunning = false;
        _timer?.Stop();
    }

    public void SetDirection(int direction) 
        => Snake.Direction = direction;

    private void Update()
    {
        if (!IsRunning || IsGameOver)
            return;

        var nextHead = Snake.GetNextHead();

        if (nextHead.Row < 0 || nextHead.Row >= GridSize ||
            nextHead.Col < 0 || nextHead.Col >= GridSize)
        {
            EndGame();
            return;
        }

        var eatingFood = Foods.FirstOrDefault(f => f.Position == nextHead);
        var eating = eatingFood != null;
        Snake.Move(eating && eatingFood!.Type != FoodType.Poison);

        if (eatingFood != null)
        {
            switch (eatingFood.Type)
            {
                case FoodType.Default:
                {
                    Player.AddScore(10);
                    eatingFood.Respawn(GridSize, Snake);

                    if (_random.NextDouble() < 0.35 && Foods.All(f => f.Type != FoodType.Gold))
                        Foods.Add(new Food(GridSize, FoodType.Gold, Snake));
                
                    if (_random.NextDouble() < 0.5 && Foods.All(f => f.Type != FoodType.Poison))
                        Foods.Add(new Food(GridSize, FoodType.Poison, Snake));
                    break;
                }
                case FoodType.Gold:
                    Player.AddScore(30);
                    Foods.Remove(eatingFood);
                    break;
                case FoodType.Poison:
                    Player.AddScore(-30);
                    Foods.Remove(eatingFood);
                    break;
            }
        }

        if (Snake.CollidesWithSelf())
        {
            EndGame();
            return;
        }

        var defaultFood = Foods.Where(food => food.Type != FoodType.Default).ToList();
        foreach (var food in defaultFood)
        {
            food.Update();
            if (food.IsExpired)
                Foods.Remove(food);
        }

        OnUpdate?.Invoke();
    }

    private void EndGame()
    {
        IsGameOver = true;
        IsRunning = false;
        _timer?.Stop();
        OnGameOver?.Invoke();
    }
}
