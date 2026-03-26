namespace Example.TicTacToe;

public class GameData(List<Player> players, int firstPlayer, int gridSize)
{
    public readonly List<Player> Players = players;
    public readonly int FirstPlayer = firstPlayer;
    public readonly int GridSize = gridSize;
}