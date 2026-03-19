namespace Example.TicTacToe;

public class Player
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public bool IsBot { get; set; }
    public int Points { get; set; }

    public Player(string name, string symbol, bool isBot)
    {
        Name = name;
        Symbol = symbol;
        IsBot = isBot;
        Points = 0;
    }
}
