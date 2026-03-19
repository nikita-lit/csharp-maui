using System;
using Microsoft.Maui.Controls;

namespace Example.TicTacToe;

public partial class TicTacToeStart : ContentPage
{
    private Entry _playerSymbolEntry;
    private Entry _opponentSymbolEntry;
    private Picker _firstPlayerPicker;
    private Picker _opponentPicker;
    private Picker _sizePicker;
    private Picker _themePicker;

    public TicTacToeStart()
    {
        InitializeComponent();
        Title = "Trips-Traps-Trull : Menüü";
        BuildUI();
    }

    private void BuildUI()
    {
        var mainLayout = new VerticalStackLayout { Padding = 20, Spacing = 20, VerticalOptions = LayoutOptions.Center };
        
        mainLayout.Children.Add(new Label { Text = "Mängu Seaded", FontSize = 32, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center });

        // Player Symbol
        var symbolLayout = new VerticalStackLayout { Spacing = 5, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250 };
        symbolLayout.Children.Add(new Label { Text = "Sinu sümbol:" });
        _playerSymbolEntry = new Entry { Text = "X", HorizontalTextAlignment = TextAlignment.Center, MaxLength = 3 };
        symbolLayout.Children.Add(_playerSymbolEntry);
        
        symbolLayout.Children.Add(new Label { Text = "Vastase sümbol:", Margin = new Thickness(0, 5, 0, 0) });
        _opponentSymbolEntry = new Entry { Text = "O", HorizontalTextAlignment = TextAlignment.Center, MaxLength = 3 };
        symbolLayout.Children.Add(_opponentSymbolEntry);
        
        mainLayout.Children.Add(symbolLayout);

        // Who starts
        var startsLayout = new VerticalStackLayout { Spacing = 5, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250 };
        startsLayout.Children.Add(new Label { Text = "Kes alustab:" });
        _firstPlayerPicker = new Picker();
        _firstPlayerPicker.Items.Add("Sina");
        _firstPlayerPicker.Items.Add("Vastane");
        _firstPlayerPicker.Items.Add("Juhuslik");
        _firstPlayerPicker.SelectedIndex = 0;
        startsLayout.Children.Add(_firstPlayerPicker);
        mainLayout.Children.Add(startsLayout);

        // Opponent
        var opponentLayout = new VerticalStackLayout { Spacing = 5, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250 };
        opponentLayout.Children.Add(new Label { Text = "Vastane:" });
        _opponentPicker = new Picker();
        _opponentPicker.Items.Add("Inimene");
        _opponentPicker.Items.Add("Bot");
        _opponentPicker.SelectedIndex = 0;
        opponentLayout.Children.Add(_opponentPicker);
        mainLayout.Children.Add(opponentLayout);

        // Size
        var sizeLayout = new VerticalStackLayout { Spacing = 5, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250 };
        sizeLayout.Children.Add(new Label { Text = "Laua suurus:" });
        _sizePicker = new Picker();
        _sizePicker.Items.Add("3x3");
        _sizePicker.Items.Add("4x4");
        _sizePicker.Items.Add("5x5");
        _sizePicker.SelectedIndex = 0;
        sizeLayout.Children.Add(_sizePicker);
        mainLayout.Children.Add(sizeLayout);

        // Theme
        var themeLayout = new VerticalStackLayout { Spacing = 5, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250 };
        themeLayout.Children.Add(new Label { Text = "Teema:" });
        _themePicker = new Picker();
        _themePicker.Items.Add("Hele");
        _themePicker.Items.Add("Tume");
        _themePicker.SelectedIndex = 0;
        themeLayout.Children.Add(_themePicker);
        mainLayout.Children.Add(themeLayout);

        // Buttons
        var startBut = new Button { Text = "Alusta mängu", BackgroundColor = Color.FromArgb("#4CAF50"), TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250, Margin = new Thickness(0, 20, 0, 0) };
        startBut.Clicked += OnStartClicked;
        
        var rulesBut = new Button { Text = "Reeglid", BackgroundColor = Color.FromArgb("#FF9800"), TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, WidthRequest = 250, Margin = new Thickness(0, 10, 0, 0) };
        rulesBut.Clicked += OnRulesClicked;

        mainLayout.Children.Add(startBut);
        mainLayout.Children.Add(rulesBut);

        Content = new ScrollView { Content = mainLayout };
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        string pSym = string.IsNullOrWhiteSpace(_playerSymbolEntry.Text) ? "X" : _playerSymbolEntry.Text.Trim();
        string oSym = string.IsNullOrWhiteSpace(_opponentSymbolEntry.Text) ? "O" : _opponentSymbolEntry.Text.Trim();
        
        if (pSym == oSym)
        {
            oSym = pSym == "X" ? "O" : "X";
        }

        int firstPlayerOpt = _firstPlayerPicker.SelectedIndex;
        bool isBot = _opponentPicker.SelectedIndex == 1;
        int size = _sizePicker.SelectedIndex == 1 ? 4 : (_sizePicker.SelectedIndex == 2 ? 5 : 3);
        bool isDark = _themePicker.SelectedIndex == 1;

        var players = new List<Player>
        {
            new Player("Mängija 1", pSym, false),
            new Player("Mängija 2", oSym, isBot),
            new Player("Mängija 3", "Z", false),
            new Player("Mängija 4", "Y", false)
        };

        await Navigation.PushAsync(new TicTacToe(players, firstPlayerOpt, size, isDark));
    }

    private async void OnRulesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TicTacToeRules());
    }
}
