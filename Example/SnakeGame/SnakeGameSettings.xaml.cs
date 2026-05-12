using System.Globalization;

namespace Example.SnakeGame;

public partial class SnakeGameSettings : ContentPage
{
    private Theme _theme;
    private string _selectedTheme;
    private int _selectedSpeed;

    public SnakeGameSettings()
    {
        InitializeComponent();

        _theme = Theme.LoadSelected();
        _selectedTheme = _theme.Name;
        _selectedSpeed = Preferences.Get("snake_speed", 200);

        PlayerNameEntry.Text = Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));

        UpdateText();
        ApplyTheme();
        HighlightSelectedTheme();
        HighlightSelectedSpeed();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
    }

    private void ApplyTheme()
    {
        _theme.Apply(this);

        SettingsTitleLabel.TextColor = _theme.TextColor;
        NameLabel.TextColor = _theme.TextColor;
        ThemeLabel.TextColor = _theme.TextColor;
        SpeedLabel.TextColor = _theme.TextColor;

        PlayerNameEntry.TextColor = _theme.TextColor;
        PlayerNameEntry.PlaceholderColor = _theme.GridColor;
        PlayerNameEntry.BackgroundColor = _theme.GridColor;

        LanguageButton.BackgroundColor = _theme.ButtonColor;
        LanguageButton.TextColor = _theme.ButtonTextColor;

        SaveButton.BackgroundColor = _theme.ButtonColor;
        SaveButton.TextColor = _theme.ButtonTextColor;

        foreach (var btn in new[] { LightBtn, DarkBtn, ColorfulBtn, SlowBtn, NormalBtn, FastBtn })
            StyleButton(btn, false);
    }

    private void HighlightSelectedTheme()
    {
        StyleButton(LightBtn, _selectedTheme == "Hele");
        StyleButton(DarkBtn, _selectedTheme == "Tume");
        StyleButton(ColorfulBtn, _selectedTheme == "Värviline");
    }

    private void HighlightSelectedSpeed()
    {
        StyleButton(SlowBtn, _selectedSpeed == 300);
        StyleButton(NormalBtn, _selectedSpeed == 200);
        StyleButton(FastBtn, _selectedSpeed == 120);
    }

    private void StyleButton(Button button, bool selected)
    {
        button.BackgroundColor = selected ? _theme.ButtonColor : _theme.GridColor;
        button.TextColor = selected ? _theme.ButtonTextColor : _theme.TextColor;
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeSettingsTitle");
        SettingsTitleLabel.Text = LanguageService.Get("SnakeSettings");
        NameLabel.Text = LanguageService.Get("SnakePlayerNameLabel");
        PlayerNameEntry.Placeholder = LanguageService.Get("SnakePlayerNamePlaceholder");
        ThemeLabel.Text = LanguageService.Get("SnakeThemeLabel");
        LightBtn.Text = LanguageService.Get("SnakeThemeLight");
        DarkBtn.Text = LanguageService.Get("SnakeThemeDark");
        ColorfulBtn.Text = LanguageService.Get("SnakeThemeColorful");
        SpeedLabel.Text = LanguageService.Get("SnakeSpeedLabel");
        SlowBtn.Text = LanguageService.Get("SnakeSpeedSlow");
        NormalBtn.Text = LanguageService.Get("SnakeSpeedNormal");
        FastBtn.Text = LanguageService.Get("SnakeSpeedFast");
        SaveButton.Text = LanguageService.Get("SnakeSave");
        LanguageButton.Text = LanguageService.Get("LanguageButton");
    }

    private void OnThemeSelected(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            _selectedTheme = btn.CommandParameter?.ToString() ?? btn.Text;
            _theme = Theme.GetByName(_selectedTheme);
            ApplyTheme();
            UpdateText();
            HighlightSelectedTheme();
            HighlightSelectedSpeed();
        }
    }

    private void OnSpeedSelected(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            _selectedSpeed = int.TryParse(btn.CommandParameter?.ToString(), out int speed) ? speed : 200;
            HighlightSelectedSpeed();
        }
    }

    private async void OnLanguageClicked(object? sender, EventArgs e)
    {
        if (sender is VisualElement btn)
        {
            await btn.ScaleToAsync(0.92, 60, Easing.CubicIn);
            await btn.ScaleToAsync(1.0, 60, Easing.CubicOut);
        }

        var code = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "et"
            ? "en-US"
            : "et-EE";

        LanguageService.ChangeLanguage(code);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var name = string.IsNullOrWhiteSpace(PlayerNameEntry.Text)
            ? LanguageService.Get("SnakeDefaultPlayer")
            : PlayerNameEntry.Text.Trim();
        Preferences.Set("snake_player_name", name);
        Theme.SaveSelected(_selectedTheme);
        Preferences.Set("snake_speed", _selectedSpeed);

        await DisplayAlertAsync(
            LanguageService.Get("SnakeSavedTitle"),
            LanguageService.Get("SnakeSavedMessage"),
            LanguageService.Get("OkButton"));
        await Navigation.PopAsync();
    }
}
