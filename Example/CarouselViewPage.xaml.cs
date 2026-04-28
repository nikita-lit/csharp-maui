using System.Collections.ObjectModel;
using System.Globalization;

namespace Example;

public partial class CarouselViewPage : ContentPage
{
    private ObservableCollection<FoodItem> _foods = [];
    private IDispatcherTimer? _autoScrollTimer;
    private bool _isAutoScrollEnabled = true;

    private readonly string[][] _foodKeys =
    [
        ["Carbonara_Name", "Carbonara_Desc", "Carbonara_Detail", "carbonara.jpg"],
        ["Pizza_Name",     "Pizza_Desc",     "Pizza_Detail", "pizza.jpg"],
        ["Tiramisu_Name",  "Tiramisu_Desc",  "Tiramisu_Detail", "tiramisu.jpg"],
        ["Risotto_Name",   "Risotto_Desc",   "Risotto_Detail", "risotto.jpg"],
        ["Lasagne_Name",   "Lasagne_Desc",   "Lasagne_Detail", "lasagne.jpg"]
    ];

    public CarouselViewPage()
    {
        InitializeComponent();
        
        for (int i = 0; i < _foodKeys.Length; i++)
            _foods.Add(new FoodItem());

        UpdateFoods();

        FoodCarousel.ItemsSource = _foods;
        LanguageService.LanguageChanged += UpdateFoods;
    }

    private void UpdateFoods()
    {
        for (int i = 0; i < _foodKeys.Length; i++)
        {
            _foods[i].Name = LanguageService.Get(_foodKeys[i][0]);
            _foods[i].Description = LanguageService.Get(_foodKeys[i][1]);
            _foods[i].DetailInfo = LanguageService.Get(_foodKeys[i][2]);
            _foods[i].ImageUrl = LanguageService.Get(_foodKeys[i][3]);
            _foods[i].TapHint = LanguageService.Get("TapForDetails");
        }

        Title = LanguageService.Get("PageTitle");
        LanguageButton.Text = LanguageService.Get("LanguageButton");
        AutoScrollLabel.Text = LanguageService.Get("AutoScroll");

        UpdateAutoScrollState();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        StartAutoScroll();
        UpdateAutoScrollState();

        if (_isAutoScrollEnabled)
            _autoScrollTimer?.Start();

        Opacity = 0;
        this.FadeToAsync(1, 600, Easing.CubicIn);
    }

    private void UpdateAutoScrollState()
    {
        AutoScrollSwitch?.IsToggled = _isAutoScrollEnabled;
    }

    private void StartAutoScroll()
    {
        _autoScrollTimer = Dispatcher.CreateTimer();
        _autoScrollTimer.Interval = TimeSpan.FromSeconds(3f);
        _autoScrollTimer.Tick += OnAutoScrollTick;
        _autoScrollTimer.Start();
    }

    private void OnAutoScrollTick(object? sender, EventArgs e)
    {
        var nextIndex = (FoodCarousel.Position + 1) % _foods.Count;
        FoodCarousel.Position = nextIndex;
    }

    private async void OnFoodTapped(object? sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleToAsync(0.96, 80, Easing.CubicIn);
            await element.ScaleToAsync(1.0, 80, Easing.CubicOut);
        }

        FoodItem? food = null;
        if (sender is VisualElement ve)
            food = ve.BindingContext as FoodItem;

        if (food != null)
        {
            await Navigation.PushAsync(new FoodDetailPage(food));
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

    private void OnAutoScrollToggled(object? sender, ToggledEventArgs e)
    {
        _isAutoScrollEnabled = e.Value;

        if (_isAutoScrollEnabled)
            _autoScrollTimer?.Start();
        else
            _autoScrollTimer?.Stop();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _autoScrollTimer?.Stop();
    }
}
