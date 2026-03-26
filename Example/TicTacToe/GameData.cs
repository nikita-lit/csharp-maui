namespace Example.TicTacToe;

public record GameData(
    List<Player> Players,
    int FirstPlayer, 
    int GridSize
);