namespace Example.SnakeGame;

public class Player(string name)
{
    public string Name { get; set; } = name;
    public int Score { get; private set; }
    public int HighScore { get; private set; } = Preferences.Get($"snake_highscore_{name}", 0);

    public void AddScore(int points)
    {
        Score += points;
        if (Score <= HighScore) 
            return;
        
        HighScore = Score;
        SaveHighScore();
    }

    public void ResetScore() 
        => Score = 0;

    private void SaveHighScore() 
        => Preferences.Set($"snake_highscore_{Name}", HighScore);
}
