namespace Example.SnakeGame;

public class Theme
{
    public string Name { get; }
    public Color BackgroundColor { get; }
    public Color TextColor { get; }
    public Color SnakeColor { get; }
    public Color SnakeHeadColor { get; }
    public Color FoodColor { get; }
    public Color GridColor { get; }
    public Color ButtonColor { get; }
    public Color ButtonTextColor { get; }
    public string FontFamily { get; }

    private Theme(string name, Color bg, Color text, Color snake, Color snakeHead,
                  Color food, Color grid, Color button, Color buttonText, string font)
    {
        Name = name;
        BackgroundColor = bg;
        TextColor = text;
        SnakeColor = snake;
        SnakeHeadColor = snakeHead;
        FoodColor = food;
        GridColor = grid;
        ButtonColor = button;
        ButtonTextColor = buttonText;
        FontFamily = font;
    }

    public static Theme Light => new Theme(
        "Hele",
        Color.FromArgb("#F5F5F5"),
        Color.FromArgb("#212121"),
        Color.FromArgb("#4CAF50"),
        Color.FromArgb("#2E7D32"),
        Color.FromArgb("#F44336"),
        Color.FromArgb("#E0E0E0"),
        Color.FromArgb("#4CAF50"),
        Colors.White,
        "Default"
    );

    public static Theme Dark => new Theme(
        "Tume",
        Color.FromArgb("#1A1A2E"),
        Color.FromArgb("#E0E0E0"),
        Color.FromArgb("#00E676"),
        Color.FromArgb("#00C853"),
        Color.FromArgb("#FF5252"),
        Color.FromArgb("#2D2D44"),
        Color.FromArgb("#00E676"),
        Color.FromArgb("#1A1A2E"),
        "Default"
    );

    public static Theme Colorful => new Theme(
        "Värviline",
        Color.FromArgb("#1B0A3C"),
        Color.FromArgb("#FFD700"),
        Color.FromArgb("#FF6EC7"),
        Color.FromArgb("#FF1493"),
        Color.FromArgb("#FFD700"),
        Color.FromArgb("#2A1052"),
        Color.FromArgb("#FF6EC7"),
        Colors.White,
        "Default"
    );

    public static List<Theme> GetAll() => new() { Light, Dark, Colorful };

    public void Apply(ContentPage page) => page.BackgroundColor = BackgroundColor;

    public static Theme GetByName(string name) => name switch
    {
        "Hele" => Light,
        "Tume" => Dark,
        "Värviline" => Colorful,
        _ => Dark
    };

    public static void SaveSelected(string name) => Preferences.Set("snake_theme", name);

    public static Theme LoadSelected() => GetByName(Preferences.Get("snake_theme", "Tume"));
}
