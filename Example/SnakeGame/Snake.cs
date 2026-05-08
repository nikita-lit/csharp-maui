namespace Example.SnakeGame;

public class Snake
{
    public const int Up = 0;
    public const int Right = 1;
    public const int Down = 2;
    public const int Left = 3;

    private readonly List<Cell> _body;
    private int _direction;

    public IReadOnlyList<Cell> Body => _body.AsReadOnly();
    public Cell Head => _body[0];

    public int Direction
    {
        get => _direction;
        set
        {
            if ((_direction == Up && value == Down) ||
                (_direction == Down && value == Up) ||
                (_direction == Left && value == Right) ||
                (_direction == Right && value == Left))
                return;
            _direction = value;
        }
    }

    public Snake(int startRow, int startCol)
    {
        _direction = Right;
        _body = new List<Cell>
        {
            new Cell(startRow, startCol),
            new Cell(startRow, startCol - 1),
            new Cell(startRow, startCol - 2)
        };
    }

    public Cell GetNextHead()
    {
        int newRow = Head.Row;
        int newCol = Head.Col;

        switch (_direction)
        {
            case Up: newRow--; break;
            case Down: newRow++; break;
            case Left: newCol--; break;
            case Right: newCol++; break;
        }

        return new Cell(newRow, newCol);
    }

    public void Move(bool grow)
    {
        Cell next = GetNextHead();
        _body.Insert(0, next);
        if (!grow)
            _body.RemoveAt(_body.Count - 1);
    }

    public bool CollidesWithSelf()
    {
        for (int i = 1; i < _body.Count; i++)
        {
            if (Head.Equals(_body[i]))
                return true;
        }
        return false;
    }

    public bool IsAt(Cell cell) => Head.Equals(cell);

    public bool Occupies(Cell cell)
    {
        foreach (var part in _body)
        {
            if (part.Equals(cell))
                return true;
        }
        return false;
    }
}
