using System.Text.Json;

namespace Example;

public class ScoreData
{
    public string Name { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
}

public partial class MultiplicationTest : ContentPage
{
    private Label _greetingLabel;
    private Label _scoreLabel;
    private string _playerName = "";
    private int _currentA, _currentB;

    public MultiplicationTest()
    {
        InitializeComponent();

        _greetingLabel = new Label
        {
            Text = "Tere tulemast!",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
        };
        
        _scoreLabel = new Label
        {
            Text = "Sinu punktid: 0",
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray
        };

        var butTest = new Button { 
            Text = "Küsi korrutustehet" 
        };
        butTest.Clicked += (s, e) => OnTestClicked();

        var butLeaderboard = new Button { 
            Text = "Kuva edetabel" 
        };
        butLeaderboard.Clicked += (s, e) => ShowLeaderboard();

        var butColor = new Button { 
            Text = "Vali teema värv" 
        };
        butColor.Clicked += (s, e) => OnColorClicked();

        var vsl = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = 30,
            VerticalOptions = LayoutOptions.Center,
            Children = { 
                _greetingLabel, 
                _scoreLabel,
                butTest, 
                butLeaderboard, 
                butColor 
            }
        };

        Content = vsl;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _currentA = Preferences.Default.Get("CurrentA", 0);
        _currentB = Preferences.Default.Get("CurrentB", 0);

        if (_currentA == 0 || _currentB == 0)
            CreateNewQuestion();

        string savedColor = Preferences.Default.Get("BgColor", "Tavaline");
        ApplyColor(savedColor);

        if (string.IsNullOrEmpty(_playerName))
            await AskName();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _playerName = "";
    }

    private void ApplyColor(string colorName)
    {
        switch (colorName)
        {
            case "Punane":
                BackgroundColor = Colors.LightPink;
                _greetingLabel.TextColor = Colors.Black;
                _scoreLabel.TextColor = Colors.DarkSlateGray;
                break;
            case "Roheline":
                BackgroundColor = Colors.LightGreen;
                _greetingLabel.TextColor = Colors.Black;
                _scoreLabel.TextColor = Colors.DarkSlateGray;
                break;
            case "Sinine":
                BackgroundColor = Colors.LightBlue;
                _greetingLabel.TextColor = Colors.Black;
                _scoreLabel.TextColor = Colors.DarkSlateGray;
                break;
            case "Tume":
                BackgroundColor = Colors.DarkSlateGray;
                _greetingLabel.TextColor = Colors.White;
                _scoreLabel.TextColor = Colors.LightGray;
                break;
            case "Tavaline":
            default:
                BackgroundColor = Colors.White;
                _greetingLabel.TextColor = Colors.Black;
                _scoreLabel.TextColor = Colors.Gray;
                break;
        }

        Preferences.Default.Set("BgColor", colorName);
    }

    private void CreateNewQuestion()
    {
        _currentA = Random.Shared.Next(1, 11);
        _currentB = Random.Shared.Next(1, 11);
        Preferences.Default.Set("CurrentA", _currentA);
        Preferences.Default.Set("CurrentB", _currentB);
    }

    private async Task AskName()
    {
        string lastPlayer = Preferences.Default.Get("LastPlayer", string.Empty);
        if (!string.IsNullOrEmpty(lastPlayer))
        {
            bool isLast = await DisplayAlertAsync("Tere!", $"Kas sa oled {lastPlayer}?", "Jah", "Ei");
            if (isLast)
                _playerName = lastPlayer;
        }

        if (string.IsNullOrEmpty(_playerName))
        {
            _playerName = await DisplayPromptAsync("Tere!", "Mis on sinu nimi?", "OK", "Loobu");
            if (string.IsNullOrWhiteSpace(_playerName))
            {
                await Navigation.PopToRootAsync();
                return;
            }
            _playerName = _playerName.Trim();
        }

        _greetingLabel.Text = $"Tere, {_playerName}!";
        Preferences.Default.Set("LastPlayer", _playerName);
        UpdateScoreLabel();
    }

    private void UpdateScoreLabel()
    {
        var json = Preferences.Default.Get("Leaderboard", "[]");
        var scores = JsonSerializer.Deserialize<List<ScoreData>>(json);
        scores ??= [];

        var score = scores.Find(x => x.Name == _playerName);
        int points = score?.Points ?? 0;

        _scoreLabel.Text = $"Sinu punktid: {points}";
    }

    private async void OnTestClicked()
    {
        bool continuePlaying = true;
        while (continuePlaying)
        {
            int answer = _currentA * _currentB;
            string userAnswerStr = await DisplayPromptAsync("Korrutamine", $"Palju on {_currentA} * {_currentB}?", keyboard: Keyboard.Numeric);

            if (userAnswerStr == null) 
                break;

            if (int.TryParse(userAnswerStr, out int userAnswer))
            {
                if (userAnswer == answer)
                {
                    await DisplayAlertAsync("Tulemus", "Õige vastus! Tubli!", "OK");
                    SaveScore(_playerName, 1);
                }
                else
                    await DisplayAlertAsync("Tulemus", $"Vale! Õige vastus on {answer}.", "OK");
                    
                CreateNewQuestion();
            }
            else
            {
                await DisplayAlertAsync("Viga", "Palun sisesta number.", "OK");
            }

            continuePlaying = await DisplayAlertAsync("Küsimus", "Kas soovid jätkata?", "Jah", "Ei");
        }
    }

    private void SaveScore(string name, int points)
    {
        var json = Preferences.Default.Get("Leaderboard", "[]");
        var scores = JsonSerializer.Deserialize<List<ScoreData>>(json);
        scores ??= [];
        
        var score = scores.Find(x => x.Name == name);

        if (score == null)
        {
            score = new ScoreData {
                Name = name,
                Points = points,
            };
            scores.Add(score);
        }
        else
            score.Points += points;
        
        Preferences.Default.Set("Leaderboard", JsonSerializer.Serialize(scores));
        UpdateScoreLabel();
    }

    private async void ShowLeaderboard()
    {
        var json = Preferences.Default.Get("Leaderboard", "[]");
        var scores = JsonSerializer.Deserialize<List<ScoreData>>(json);

        if (scores == null || scores.Count == 0)
        {
            await DisplayAlertAsync("Edetabel", "Edetabel on tühi.", "OK");
            return;
        }

        var topScores = scores
            .OrderByDescending(x => x.Points)
            .Take(5);

        string result = "";
        foreach (var s in topScores.Index())
        {
            result += $"{s.Index + 1}. {s.Item.Name}: {s.Item.Points} punkti\n";
        }

        await DisplayAlertAsync("Edetabel", result, "OK");
    }

    private async void OnColorClicked()
    {
        string choice = await DisplayActionSheetAsync("Vali teema värv", "Loobu", null, "Punane", "Roheline", "Sinine", "Tume", "Tavaline");
        if (!string.IsNullOrEmpty(choice) && choice != "Loobu")
            ApplyColor(choice);
    }
}