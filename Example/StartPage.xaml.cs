using Example.TicTacToe;
using Example.SnakeGame;
using Example.RecipeBook;
using Microsoft.Maui.Controls.Shapes;

namespace Example;

public class MenuEntry
{
    public string Title { get; set; }
    public Type PageType { get; set; }
}

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        Title = "App Menu";
        BackgroundColor = Color.FromArgb("#F5F5F7");

        var menuItems = new List<MenuEntry>
        {
            new() { Title = "Tekst", PageType = typeof(TextPage) },
            new() { Title = "Kujund", PageType = typeof(FigurePage) },
            new() { Title = "Valgusfoor", PageType = typeof(ValgusfoorPage) },
            new() { Title = "RGB", PageType = typeof(RGBPage) },
            new() { Title = "Lumememm", PageType = typeof(LumememmPage) },
            new() { Title = "Matemaatika", PageType = typeof(MathTest) },
            new() { Title = "Trips Traps Trull", PageType = typeof(TicTacToeStart) },
            new() { Title = "Snake mäng", PageType = typeof(SnakeGameStart) },
            new() { Title = "Retseptid", PageType = typeof(RecipeBookPage) },
        };
        
        var grid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition() },
            ColumnSpacing = 5,
            RowSpacing = 5,
            Padding = 20
        };

        for (var i = 0; i < menuItems.Count; i++)
        {
            var item = menuItems[i];
            var card = CreateMenuCard(item);
            
            grid.Add(card, i % 2, i / 2); 
        }

        Content = new ScrollView { Content = grid };
    }

    public View CreateMenuCard(MenuEntry item)
    {
        var border = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            StrokeThickness = 0,
            HeightRequest = 100,
            Content = new Label
            {
                Text = item.Title,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            },
        };
        
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
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
