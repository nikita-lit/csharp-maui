namespace Example.SnakeGame;

public class Food
{
    private readonly Random _random = new();

    public Cell Position { get; private set; }

    public Food(int gridSize, Snake snake)
    {
        Position = new Cell(0, 0);
        Respawn(gridSize, snake);
    }

    public void Respawn(int gridSize, Snake snake)
    {
        do
        {
            Position = new Cell(_random.Next(gridSize), _random.Next(gridSize));
        }
        while (snake.Occupies(Position));
    }
}
