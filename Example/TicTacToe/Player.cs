namespace Example.TicTacToe;

public class Player(string name, string symbol, bool isBot)
{
    public string Name { get; set; } = name;
    public string Symbol { get; set; } = symbol;
    public bool IsBot { get; set; } = isBot;
    public int Points { get; set; } = 0;
}
