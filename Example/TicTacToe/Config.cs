namespace Example.TicTacToe;

public static class Config
{
    public static string Player1Name
    {
        get => Preferences.Get("Player1Name", "Mängija 1");
        set => Preferences.Set("Player1Name", value);
    }
    public static string Player1Symbol
    {
        get => Preferences.Get("Player1Symbol", "X");
        set => Preferences.Set("Player1Symbol", value);
    }

    public static string Player2Name
    {
        get => Preferences.Get("Player2Name", "Mängija 2");
        set => Preferences.Set("Player2Name", value);
    }
    public static string Player2Symbol
    {
        get => Preferences.Get("Player2Symbol", "O");
        set => Preferences.Set("Player2Symbol", value);
    }

    public static int FirstPlayerIndex
    {
        get => Preferences.Get("FirstPlayerIndex", 0);
        set => Preferences.Set("FirstPlayerIndex", value);
    }
    public static int GridSize
    {
        get => Preferences.Get("GridSize", 3);
        set => Preferences.Set("GridSize", value);
    }

    public static bool IsTimerEnabled
    {
        get => Preferences.Get("IsTimerEnabled", false);
        set => Preferences.Set("IsTimerEnabled", value);
    }
    public static int TimerSeconds
    {
        get => Preferences.Get("TimerSeconds", 5);
        set => Preferences.Set("TimerSeconds", value);
    }
}
