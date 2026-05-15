namespace Example.SnakeGame.Models;

public class Cell(int row, int col, BoxView boxView)
{
    public int Row { get; } = row;
    public int Col { get; } = col;
    public BoxView BoxView { get; } = boxView;

    public void SetColor(Color color) 
        => BoxView.Color = color;
}