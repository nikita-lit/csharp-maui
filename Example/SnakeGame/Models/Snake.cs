namespace Example.SnakeGame.Models;

public class Snake(int startRow, int startCol)
{
    public const int Up = 0;
    public const int Right = 1;
    public const int Down = 2;
    public const int Left = 3;

    private readonly List<(int Row, int Col)> _body =
    [
        (startRow, startCol),
        (startRow, startCol - 1),
        (startRow, startCol - 2)
    ];
    
    private int _direction = Right;

    public IReadOnlyList<(int Row, int Col)> Body 
        => _body.AsReadOnly();
    
    public (int Row, int Col) Head 
        => _body[0];

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

    public (int Row, int Col) GetNextHead()
    {
        var newRow = Head.Row;
        var newCol = Head.Col;

        switch (_direction)
        {
            case Up: newRow--; break;
            case Down: newRow++; break;
            case Left: newCol--; break;
            case Right: newCol++; break;
        }

        return (newRow, newCol);
    }

    public void Move(bool grow)
    {
        var next = GetNextHead();
        _body.Insert(0, next);
        if (!grow)
            _body.RemoveAt(_body.Count - 1);
    }

    public bool CollidesWithSelf()
    {
        for (var i = 1; i < _body.Count; i++)
        {
            if (Head.Equals(_body[i]))
                return true;
        }
        
        return false;
    }

    public bool Occupies((int Row, int Col) pos) 
        => _body.Contains(pos);
}
