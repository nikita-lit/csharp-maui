namespace Example.SnakeGame;

public class Game
{
    private IDispatcherTimer? _timer;

    public event Action? OnUpdate;
    public event Action? OnGameOver;

    public Snake Snake { get; }
    public Food Food { get; }
    public Player Player { get; }
    public Theme Theme { get; }
    public int GridSize { get; }
    public int Speed { get; }
    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }

    public Game(Player player, Theme theme, int gridSize = 15, int speed = 200)
    {
        Player = player;
        Theme = theme;
        GridSize = gridSize;
        Speed = speed;
        Snake = new Snake(gridSize / 2, gridSize / 2);
        Food = new Food(gridSize, Snake);
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

        var eating = nextHead.Equals(Food.Position);
        Snake.Move(eating);

        if (eating)
        {
            Player.AddScore(10);
            Food.Respawn(GridSize, Snake);
        }

        if (Snake.CollidesWithSelf())
        {
            EndGame();
            return;
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
