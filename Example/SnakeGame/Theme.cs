namespace Example.SnakeGame;

public class Theme
{
    public string Name { get; set; }
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }
    public Color SnakeColor { get; set; }
    public Color SnakeHeadColor { get; set; }
    public Color FoodColor { get; set; }
    public Color GoldColor { get; set; }
    public Color PoisonColor { get; set; }
    public Color GridColor { get; set; }
    public Color ButtonColor { get; set; }
    public Color ButtonTextColor { get; set; }
    public string FontFamily { get; set; }

    public Theme(string name, Color bg, Color text, Color snake, Color snakeHead,
                 Color food, Color gold, Color poison, Color grid, Color button, Color buttonText, string fontFamily)
    {
        Name = name;
        BackgroundColor = bg;
        TextColor = text;
        SnakeColor = snake;
        SnakeHeadColor = snakeHead;
        FoodColor = food;
        GoldColor = gold;
        PoisonColor = poison;
        GridColor = grid;
        ButtonColor = button;
        ButtonTextColor = buttonText;
        FontFamily = fontFamily;
    }

    public override string ToString() 
        => Name;

    public void Apply(ContentPage page)
    {
        page.BackgroundColor = BackgroundColor;
        
        Application.Current.Resources["GlobalBorderStyle"] = new Style(typeof(Border))
        {
            Setters =
            {
                new Setter { Property = Border.BackgroundColorProperty, Value = GridColor },
            }
        };

        Application.Current.Resources["GlobalLabelStyle"] = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontFamilyProperty, Value = FontFamily },
                new Setter { Property = Label.TextColorProperty, Value = TextColor }
            }
        };

        Application.Current.Resources["GlobalEntryStyle"] = new Style(typeof(Entry))
        {
            Setters =
            {
                new Setter { Property = Entry.FontFamilyProperty, Value = FontFamily },
                new Setter { Property = Entry.TextColorProperty, Value = TextColor },
                new Setter { Property = Entry.BackgroundColorProperty, Value = GridColor },
                new Setter { Property = Entry.PlaceholderColorProperty, Value = GridColor }
            }
        };

        Application.Current.Resources["GlobalButtonStyle"] = new Style(typeof(Button))
        {
            Setters =
            {
                new Setter { Property = Button.FontFamilyProperty, Value = FontFamily },
                new Setter { Property = Button.TextColorProperty, Value = ButtonTextColor },
                new Setter { Property = Button.BackgroundColorProperty, Value = ButtonColor }
            }
        };
    }

    public static Theme Light => new(
        "Hele",
        Color.FromArgb("#F5F5F5"),
        Color.FromArgb("#212121"),
        Color.FromArgb("#4CAF50"),
        Color.FromArgb("#2E7D32"),
        Color.FromArgb("#FF5252"),
        Color.FromArgb("#FCFF52"),
        Color.FromArgb("#517341"),
        Color.FromArgb("#c2c2c2"),
        Color.FromArgb("#4CAF50"),
        Colors.White,
        "OpenSansRegular"
    );

    public static Theme Dark => new(
        "Tume",
        Color.FromArgb("#1A1A2E"),
        Color.FromArgb("#E0E0E0"),
        Color.FromArgb("#00E676"),
        Color.FromArgb("#00C853"),
        Color.FromArgb("#FF5252"),
        Color.FromArgb("#FCFF52"),
        Color.FromArgb("#517341"),
        Color.FromArgb("#2D2D44"),
        Color.FromArgb("#00E676"),
        Color.FromArgb("#1A1A2E"),
        "OpenSansRegular"
    );

    public static Theme Colorful => new(
        "Värviline",
        Color.FromArgb("#1B0A3C"),
        Color.FromArgb("#FFD700"),
        Color.FromArgb("#FF6EC7"),
        Color.FromArgb("#FF1493"),
        Color.FromArgb("#FF5252"),
        Color.FromArgb("#FCFF52"),
        Color.FromArgb("#517341"),
        Color.FromArgb("#2A1052"),
        Color.FromArgb("#FF6EC7"),
        Colors.White,
        "ComicSansMSRegular"
    );
    
    public static Theme GetByName(string name) => name switch
    {
        "Hele" => Light,
        "Tume" => Dark,
        "Värviline" => Colorful,
        _ => Dark
    };

    public static void SaveSelected(string name) 
        => Preferences.Set("snake_theme", name);

    public static Theme LoadSelected() 
        => GetByName(Preferences.Get("snake_theme", "Tume"));
    
    public static Theme Current { get; private set; } = LoadSelected();

    public static void ChangeTheme(string themeName)
    {
        Current = GetByName(themeName);
        SaveSelected(Current.Name);
    }
}
