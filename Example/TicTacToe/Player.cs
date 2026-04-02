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
        string? opponentSymbol = null;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (string.IsNullOrEmpty(board[x, y]))
                    emptyCells.Add((x, y));
                else if (board[x, y] != Symbol)
                    opponentSymbol = board[x, y];
            }
        }

        if (emptyCells.Count == 0)
            return null;

        // win
        foreach (var (r, c) in emptyCells)
        {
            if (WillMoveWin(r, c, gridSize, board, Symbol))
                return (r, c);
        }

        // block
        int ch = Random.Shared.Next(0, 10);
        if (opponentSymbol != null && ch > 3)
        {
            foreach (var (r, c) in emptyCells)
            {
                if (WillMoveWin(r, c, gridSize, board, opponentSymbol))
                    return (r, c);
            }
        }

        return emptyCells[Random.Shared.Next(emptyCells.Count)];
    }

    private bool WillMoveWin(int x, int y, int gridSize, string[,] board, string symbol)
    {
        // row check
        bool rowWin = true;
        for (int j = 0; j < gridSize; j++)
        {
            string current = (j == y) ? symbol : board[x, j];
            if (current != symbol)
            {
                rowWin = false;
                break;
            }
        }

        if (rowWin)
            return true;

        // col check
        bool colWin = true;
        for (int i = 0; i < gridSize; i++)
        {
            string current = (i == x) ? symbol : board[i, y];
            if (current != symbol)
            {
                colWin = false;
                break;
            }
        }

        if (colWin)
            return true;

        // diag 1 
        if (x == y)
        {
            bool diagWin = true;
            for (int i = 0; i < gridSize; i++)
            {
                string current = (i == x) ? symbol : board[i, i];
                if (current != symbol)
                {
                    diagWin = false;
                    break;
                }
            }

            if (diagWin)
                return true;
        }

        // diag 2
        if (x + y == gridSize - 1)
        {
            bool diagWin = true;
            for (int i = 0; i < gridSize; i++)
            {
                string current = (i == x) ? symbol : board[i, gridSize - 1 - i];
                if (current != symbol)
                {
                    diagWin = false;
                    break;
                }
            }

            if (diagWin)
                return true;
        }

        return false;
    }
}
