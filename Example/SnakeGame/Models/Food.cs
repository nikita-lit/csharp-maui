namespace Example.SnakeGame.Models;

public enum FoodType
{
    Default,
    Gold,
    Poison
}

public class Food
{
    private readonly Random _random = new();

    public (int, int) Position { get; private set; }
    public FoodType Type { get; private set; }
    private int _movesLeft;

    public Food(int gridSize, FoodType type, Snake snake)
    {
        Type = type;
        Respawn(gridSize, snake);
    }

    public void Respawn(int gridSize, Snake snake)
    {
        do
        {
            Position = (_random.Next(gridSize), _random.Next(gridSize));
        }
        while (snake.Occupies(Position));

        _movesLeft = Type switch
        {
            FoodType.Gold => 20,
            FoodType.Poison => 30,
            _ => _movesLeft
        };
    }

    public void Update()
    {
        if (Type != FoodType.Default)
            _movesLeft--;
    }

    public bool IsExpired 
        => (Type != FoodType.Default) && _movesLeft <= 0;
}
