namespace Example.TicTacToe;

public partial class TicTacToeRules : ContentPage
{
    public TicTacToeRules()
    {
        InitializeComponent();
        Title = "Trips-Traps-Trull : Reeglid";

        var mainLayout = new VerticalStackLayout { 
            Padding = 20, 
            Spacing = 15 
        };

        mainLayout.Children.Add(new Label { 
            Text = "Mängu Reeglid", 
            FontSize = 32, FontAttributes = FontAttributes.Bold, 
            HorizontalOptions = LayoutOptions.Center 
        });
        mainLayout.Children.Add(new Label { 
            Text = "1. Mäng toimub ruudustikul 3x3 (või 4x4, 5x5).", 
            FontSize = 16 
        });
        mainLayout.Children.Add(new Label { 
            Text = "2. Kaks mängijat (X ja O) teevad kordamööda käike.", 
            FontSize = 16 
        });
        mainLayout.Children.Add(new Label { 
            Text = "3. Eesmärk on saada oma sümbolid (vastavalt ruudustiku suurusele) järjestikku, kas horisontaalselt, vertikaalselt või diagonaalselt.", 
            FontSize = 16 
        });
        mainLayout.Children.Add(new Label { 
            Text = "4. Esimene mängija, kes saavutab järjestikused sümbolid võidab.", 
            FontSize = 16 
        });
        mainLayout.Children.Add(new Label { 
            Text = "5. Kui kõik ruudud on täidetud ja keegi ei ole võitnud, on tegemist viigiga.",
            FontSize = 16 
        });
        mainLayout.Children.Add(new Label { 
            Text = "Edu ja head mängimist!", 
            FontSize = 20, FontAttributes = FontAttributes.Bold, 
            HorizontalOptions = LayoutOptions.Center, 
            Margin = new Thickness(0, 20, 0, 0) 
        });
        
        var backBut = new Button { 
            Text = "Tagasi mängu", 
            BackgroundColor = Color.FromArgb("#2196F3"), 
            TextColor = Colors.White, Margin = new Thickness(0, 20, 0, 0) 
        };
        backBut.Clicked += async (e, sender) => {
            await Navigation.PopAsync();
        };
        mainLayout.Children.Add(backBut);
        
        Content = new ScrollView { 
            Content = mainLayout 
        };
    }
}
