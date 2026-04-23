using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Example;

public partial class CarouselViewPage : ContentPage
{
    private ObservableCollection<FoodItem> _foods = new();
    private IDispatcherTimer? _autoScrollTimer;
    private bool _isAutoScrollEnabled = true;
    private readonly ResourceManager _resourceManager;

    private readonly string[] _imageUrls =
    [
        "https://images.unsplash.com/photo-1612874742237-6526221588e3?w=600&h=400&fit=crop",
        "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=600&h=400&fit=crop",
        "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=600&h=400&fit=crop",
        "https://images.unsplash.com/photo-1476124369491-e7addf5db371?w=600&h=400&fit=crop",
        "https://images.unsplash.com/photo-1574894709920-11b28e7367e3?w=600&h=400&fit=crop"
    ];

    private readonly string[][] _foodKeys =
    [
        ["Carbonara_Name", "Carbonara_Desc", "Carbonara_Detail"],
        ["Pizza_Name",     "Pizza_Desc",     "Pizza_Detail"],
        ["Tiramisu_Name",  "Tiramisu_Desc",  "Tiramisu_Detail"],
        ["Risotto_Name",   "Risotto_Desc",   "Risotto_Detail"],
        ["Lasagne_Name",   "Lasagne_Desc",   "Lasagne_Detail"]
    ];

    public CarouselViewPage()
    {
        InitializeComponent();

        _resourceManager = new ResourceManager(
            "Example.Resources.Localization.AppResources",
            Assembly.GetExecutingAssembly());

        LoadFoods();
        StartAutoScroll();

        Opacity = 0;
        this.FadeToAsync(1, 600, Easing.CubicIn);
    }

    private void LoadFoods()
    {
        int currentPosition = FoodCarousel.Position;

        string tapHint = GetString("TapForDetails");

        _foods = new ObservableCollection<FoodItem>();
        for (int i = 0; i < _foodKeys.Length; i++)
        {
            _foods.Add(new FoodItem
            {
                Name = GetString(_foodKeys[i][0]),
                Description = GetString(_foodKeys[i][1]),
                ImageUrl = _imageUrls[i],
                DetailInfo = GetString(_foodKeys[i][2]),
                TapHint = tapHint
            });
        }

        FoodCarousel.ItemsSource = _foods;

        if (currentPosition >= 0 && currentPosition < _foods.Count)
            FoodCarousel.Position = currentPosition;

        Title = GetString("PageTitle");
        LanguageButton.Text = GetString("LanguageButton");

        UpdateAutoScrollState();
    }

    private void UpdateAutoScrollState()
    {
        if (_isAutoScrollEnabled)
        {
            AutoScrollIcon.Source = "pause.png";
        }
        else
        {
            AutoScrollIcon.Source = "play.png";
        }
    }

    private string GetString(string key)
    {
        return _resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }

    private void StartAutoScroll()
    {
        _autoScrollTimer = Dispatcher.CreateTimer();
        _autoScrollTimer.Interval = TimeSpan.FromSeconds(2.5f);
        _autoScrollTimer.Tick += OnAutoScrollTick;
        _autoScrollTimer.Start();
    }

    private void OnAutoScrollTick(object? sender, EventArgs e)
    {
        if (_foods.Count == 0) 
            return;

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
            string title = GetString("DetailsTitle");
            await DisplayAlertAsync(
                $"🍽️ {food.Name}",
                food.DetailInfo,
                "OK");
        }
    }

    private async void OnLanguageClicked(object? sender, EventArgs e)
    {
        if (sender is VisualElement btn)
        {
            await btn.ScaleToAsync(0.92, 60, Easing.CubicIn);
            await btn.ScaleToAsync(1.0, 60, Easing.CubicOut);
        }

        var newCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "et" 
            ? new CultureInfo("en-US") 
            : new CultureInfo("et-EE");

        CultureInfo.DefaultThreadCurrentCulture = newCulture;
        CultureInfo.DefaultThreadCurrentUICulture = newCulture;
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;

        LoadFoods();
    }

    private async void OnAutoScrollToggle(object? sender, EventArgs e)
    {
        if (sender is VisualElement btn)
        {
            await btn.ScaleToAsync(0.92, 60, Easing.CubicIn);
            await btn.ScaleToAsync(1.0, 60, Easing.CubicOut);
        }

        _isAutoScrollEnabled = !_isAutoScrollEnabled;

        if (_isAutoScrollEnabled)
            _autoScrollTimer?.Start();
        else
            _autoScrollTimer?.Stop();

        UpdateAutoScrollState();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _autoScrollTimer?.Stop();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_isAutoScrollEnabled)
            _autoScrollTimer?.Start();
    }
}
