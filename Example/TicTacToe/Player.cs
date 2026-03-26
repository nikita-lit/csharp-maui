namespace Example.TicTacToe;

public class Player(string name, string symbol, bool isBot)
{
    public readonly string Name = name;
    public readonly string Symbol = symbol;
    public readonly bool IsBot = isBot;
    public int Points { get; set; } = 0;

    public (int x, int y)? GetBotMove(int gridSize, string[,] board)
    {
        if (!IsBot) 
            return null;

        var emptyCells = new List<(int r, int c)>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (string.IsNullOrEmpty(board[x, y]))
                    emptyCells.Add((x, y));
            }
        }

        if (emptyCells.Count > 0)
            return emptyCells[Random.Shared.Next(emptyCells.Count)];

        return null;
    }
}

