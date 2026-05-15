using System.Globalization;
using Example.SnakeGame.ViewModels;

namespace Example.SnakeGame;

public partial class SnakeGameSettings : ContentPage
{
    private readonly SnakeSettingsViewModel _viewModel = new();
    private Theme _appliedTheme = Theme.Current;
    private string _previousLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    
    public SnakeGameSettings()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        
        LanguageService.LanguageChanged -= UpdateText;
        LanguageService.LanguageChanged += UpdateText;

        UpdateText();
        ApplyTheme(Theme.Current);
        
        _viewModel.Load();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= UpdateText;
    }

    private void ApplyTheme(Theme theme)
    {
        _appliedTheme = theme;
        theme.Apply(this);
        HighlightSelectedTheme();
        HighlightSelectedSpeed();
    }

    private void HighlightSelectedTheme()
    {
        StyleButton(LightBtn, _viewModel.SelectedTheme == "Hele");
        StyleButton(DarkBtn, _viewModel.SelectedTheme == "Tume");
        StyleButton(ColorfulBtn, _viewModel.SelectedTheme == "Värviline");
    }

    private void HighlightSelectedSpeed()
    {
        StyleButton(SlowBtn, _viewModel.SelectedSpeed == 300);
        StyleButton(NormalBtn, _viewModel.SelectedSpeed == 200);
        StyleButton(FastBtn, _viewModel.SelectedSpeed == 120);
    }

    private void StyleButton(Button button, bool selected)
    {
        button.BackgroundColor = selected ? _appliedTheme.ButtonColor : _appliedTheme.GridColor;
        button.TextColor = selected ? _appliedTheme.ButtonTextColor : _appliedTheme.TextColor;
    }

    private void UpdateText()
    {
        Title = LanguageService.Get("SnakeSettingsTitle");
        SettingsTitleLabel.Text = LanguageService.Get("SnakeSettings");
        LanguageLabel.Text = LanguageService.Get("LanguageLabel");
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
        if (sender is not Button btn) 
            return;
        
        _viewModel.SelectedTheme = btn.CommandParameter?.ToString() ?? btn.Text;
        var selectedTheme = Theme.GetByName(_viewModel.SelectedTheme);
        ApplyTheme(selectedTheme);
        UpdateText();
    }

    private void OnSpeedSelected(object? sender, EventArgs e)
    {
        if (sender is not Button btn) 
            return;
        
        _viewModel.SelectedSpeed = int.TryParse(btn.CommandParameter?.ToString(), out int speed) ? speed : 200;
        HighlightSelectedSpeed();
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

        _previousLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        LanguageService.ChangeLanguage(code);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        _viewModel.Save();

        await DisplayAlertAsync(
            LanguageService.Get("SnakeSavedTitle"),
            LanguageService.Get("SnakeSavedMessage"),
            LanguageService.Get("OkButton"));
        await Navigation.PopAsync();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();
}
