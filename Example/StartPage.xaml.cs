using Example.TicTacToe;
using Example.SnakeGame;
using Example.RecipeBook;
using Microsoft.Maui.Controls.Shapes;

namespace Example;

public class MenuEntry
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public Type PageType { get; set; }
}

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        Title = "Peamenüü";
        BackgroundColor = Color.FromArgb("#F2F2F7");

        var menuItems = new List<MenuEntry>
        {
            new() { Title = "Tekst", Icon = "📝", PageType = typeof(TextPage) },
            new() { Title = "Kujund", Icon = "📐", PageType = typeof(FigurePage) },
            new() { Title = "Valgusfoor", Icon = "🚦", PageType = typeof(ValgusfoorPage) },
            new() { Title = "RGB", Icon = "🎨", PageType = typeof(RGBPage) },
            new() { Title = "Lumememm", Icon = "☃️", PageType = typeof(LumememmPage) },
            new() { Title = "Matemaatika", Icon = "🧮", PageType = typeof(MathTest) },
            new() { Title = "Trips Traps Trull", Icon = "❌", PageType = typeof(TicTacToeStart) },
            new() { Title = "Snake mäng", Icon = "🐍", PageType = typeof(SnakeGameStart) },
            new() { Title = "Retseptid", Icon = "🍲", PageType = typeof(RecipeBookPage) },
        };

        var grid = new Grid
        {
            ColumnDefinitions = 
            { 
                new ColumnDefinition { Width = GridLength.Star }, 
                new ColumnDefinition { Width = GridLength.Star } 
            },
            ColumnSpacing = 8,
            RowSpacing = 8,
            Padding = new Thickness(20, 20, 20, 30)
        };
        
        var rowCount = (int)Math.Ceiling(menuItems.Count / 2.0);
        for (var i = 0; i < rowCount; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (var i = 0; i < menuItems.Count; i++)
        {
            var item = menuItems[i];
            var card = CreateMenuCard(item);
            
            grid.Add(card, i % 2, i / 2); 
        }

        Content = new ScrollView 
        { 
            Content = grid,
            VerticalScrollBarVisibility = ScrollBarVisibility.Never
        };
    }

    public View CreateMenuCard(MenuEntry item)
    {
        var cardContent = new StackLayout
        {
            Spacing = 8,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        cardContent.Children.Add(new Label
        {
            Text = item.Icon,
            FontSize = 28,
            HorizontalOptions = LayoutOptions.Center
        });

        cardContent.Children.Add(new Label
        {
            Text = item.Title,
            TextColor = Color.FromArgb("#1C1C1E"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            LineBreakMode = LineBreakMode.WordWrap,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Center
        });

        var border = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            StrokeThickness = 0,
            BackgroundColor = Colors.White,
            HeightRequest = 110,
            Padding = 12,
            Content = cardContent,
            Shadow = new Shadow
            {
                Offset = new Point(0, 4),
                Radius = 8,
                Opacity = 0.08f,
                Brush = Colors.Black
            }
        };
        
        var tapGesture = new TapGestureRecognizer();
        
        tapGesture.Tapped += async (s, e) =>
        {
            if (s is Border b)
            {
                await b.ScaleToAsync(0.95, 50, Easing.CubicOut);
                await b.ScaleToAsync(1.0, 50, Easing.CubicIn);
            }

            try
            {
                var page = (ContentPage)Activator.CreateInstance(item.PageType)!;
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Viga", $"Ei saa avada: {ex.Message}", "OK");
            }
        };

        border.GestureRecognizers.Add(tapGesture);
        return border;
    }
}