namespace Example.SnakeGame;

public static class ThemeService
{
    public static event Action<Theme>? ThemeChanged;

    public static Theme CurrentTheme { get; private set; } = Theme.LoadSelected();

    public static void ChangeTheme(string themeName)
    {
        CurrentTheme = Theme.GetByName(themeName);
        Theme.SaveSelected(CurrentTheme.Name);
        ThemeChanged?.Invoke(CurrentTheme);
    }

    public static void Reload()
    {
        CurrentTheme = Theme.LoadSelected();
        ThemeChanged?.Invoke(CurrentTheme);
    }
}
