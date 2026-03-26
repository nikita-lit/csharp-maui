namespace Example.TicTacToe;

public partial class TicTacToeStart : ContentPage
{
    private Entry _playerSymbolEntry;
    private Entry _opponentSymbolEntry;
    private Picker _firstPlayerPicker;
    private Picker _opponentPicker;

    public TicTacToeStart()
    {
        InitializeComponent();
        Title = "Trips-Traps-Trull : Menüü";

        var mainLayout = new VerticalStackLayout { 
            Padding = 20, 
            Spacing = 20, 
            VerticalOptions = LayoutOptions.Center 
        };
        
        mainLayout.Children.Add(new Label { 
            Text = "Mängu Seaded", 
            FontSize = 32, 
            FontAttributes = FontAttributes.Bold, 
            HorizontalOptions = LayoutOptions.Center 
        });

        var symbolLayout = new VerticalStackLayout { 
            Spacing = 5, 
            HorizontalOptions = LayoutOptions.Center, 
            WidthRequest = 250 
        };
        symbolLayout.Children.Add(new Label { 
            Text = "Sinu sümbol:" 
        });
        _playerSymbolEntry = new Entry { 
            Text = "X",
            HorizontalTextAlignment = TextAlignment.Center, 
            MaxLength = 3 
        };
        symbolLayout.Children.Add(_playerSymbolEntry);
        
        symbolLayout.Children.Add(new Label { 
            Text = "Vastase sümbol:", 
            Margin = new Thickness(0, 5, 0, 0)
        });
        _opponentSymbolEntry = new Entry { 
            Text = "O", 
            HorizontalTextAlignment = TextAlignment.Center, 
            MaxLength = 3 
        };
        symbolLayout.Children.Add(_opponentSymbolEntry);
        
        mainLayout.Children.Add(symbolLayout);

        var startsLayout = new VerticalStackLayout { 
            Spacing = 5, 
            HorizontalOptions = LayoutOptions.Center, 
            WidthRequest = 250 
        };
        startsLayout.Children.Add(new Label { 
            Text = "Kes alustab:" 
        });
        _firstPlayerPicker = new Picker();
        _firstPlayerPicker.Items.Add("Sina");
        _firstPlayerPicker.Items.Add("Vastane");
        _firstPlayerPicker.Items.Add("Juhuslik");
        _firstPlayerPicker.SelectedIndex = 0;
        startsLayout.Children.Add(_firstPlayerPicker);
        mainLayout.Children.Add(startsLayout);

        var opponentLayout = new VerticalStackLayout { 
            Spacing = 5, 
            HorizontalOptions = LayoutOptions.Center, 
            WidthRequest = 250
        };
        opponentLayout.Children.Add(new Label { 
            Text = "Vastane:" 
        });
        _opponentPicker = new Picker();
        _opponentPicker.Items.Add("Inimene");
        _opponentPicker.Items.Add("Bot");
        _opponentPicker.SelectedIndex = 0;
        opponentLayout.Children.Add(_opponentPicker);
        mainLayout.Children.Add(opponentLayout);

        var startBut = new Button { 
            Text = "Alusta mängu", 
            BackgroundColor = Color.FromArgb("#4CAF50"), 
            TextColor = Colors.White, 
            HorizontalOptions = LayoutOptions.Center, 
            WidthRequest = 250, 
            Margin = new Thickness(0, 20, 0, 0) 
        };
        startBut.Clicked += OnStartClicked;
        mainLayout.Children.Add(startBut);

        Content = new ScrollView { 
            Content = mainLayout 
        };
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        string pSym = string.IsNullOrWhiteSpace(_playerSymbolEntry.Text) ? "X" : _playerSymbolEntry.Text.Trim();
        string oSym = string.IsNullOrWhiteSpace(_opponentSymbolEntry.Text) ? "O" : _opponentSymbolEntry.Text.Trim();
        
        if (pSym == oSym)
            oSym = pSym == "X" ? "O" : "X";

        int first = _firstPlayerPicker.SelectedIndex;
        bool isBot = _opponentPicker.SelectedIndex == 1;

        var players = new List<Player>
        {
            new Player("Mängija 1", pSym, false),
            new Player("Mängija 2", oSym, isBot),
        };

        await Navigation.PushAsync(new TicTacToe(
            new GameData(players, first, 3)
        ));
    }
}
