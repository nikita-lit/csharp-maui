using System.Collections.ObjectModel;
using System.Windows.Input;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class ExploreViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly LocalizationService _localizationService;
    private readonly List<Place> _allPlaces;
    private Category? _selectedCategory;

    public ExploreViewModel(DatabaseService databaseService, LocalizationService localizationService)
    {
        _databaseService = databaseService;
        _localizationService = localizationService;
        _allPlaces = PlaceCatalog.CreateSeedPlaces();

        Places = [];
        Categories = [];
        LoadPlacesCommand = new Command(LoadPlaces);
        AddToFavoritesCommand = new Command<Place>(async place => await AddToFavoritesAsync(place));

        _localizationService.LanguageChanged += (_, _) => RefreshLocalizedText();
        LoadCategories();
        SelectedCategory = Categories.FirstOrDefault();
    }

    public ObservableCollection<Place> Places { get; }
    public ObservableCollection<Category> Categories { get; }
    public ICommand LoadPlacesCommand { get; }
    public ICommand AddToFavoritesCommand { get; }

    public string ExploreTitle => _localizationService["ExploreTitle"];
    public string PageHeading => _localizationService["ExploreHeading"];
    public string PageSubtitle => _localizationService["ExploreSubtitle"];

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                UpdateCategorySelection();
                LoadPlaces();
            }
        }
    }

    public string AddFavoriteText => _localizationService["AddToFavorites"];
    public string FavoriteSavedText => _localizationService["FavoriteSaved"];
    public string OkText => _localizationService["Ok"];

    private void LoadCategories()
    {
        var selectedKey = SelectedCategory?.Key ?? "Historical";
        Categories.Clear();
        Categories.Add(new Category { Emoji = "🏰", Key = "Historical", Title = _localizationService["CategoryHistorical"] });
        Categories.Add(new Category { Emoji = "🌳", Key = "Parks", Title = _localizationService["CategoryParks"] });
        Categories.Add(new Category { Emoji = "🍽️", Key = "Restaurants", Title = _localizationService["CategoryRestaurants"] });
        SelectedCategory = Categories.FirstOrDefault(category => category.Key == selectedKey) ?? Categories.FirstOrDefault();
        UpdateCategorySelection();
    }

    private void UpdateCategorySelection()
    {
        foreach (var category in Categories)
            category.IsSelected = category.Key == SelectedCategory?.Key;
    }

    private void LoadPlaces()
    {
        Places.Clear();
        var categoryKey = SelectedCategory?.Key ?? "Historical";

        foreach (var place in _allPlaces.Where(place => place.Category == categoryKey))
            Places.Add(PlaceCatalog.LocalizePlace(place, _localizationService));
    }

    private async Task AddToFavoritesAsync(Place? place)
    {
        if (place is null)
            return;

        await _databaseService.AddFavoriteAsync(place);
    }

    private void RefreshLocalizedText()
    {
        LoadCategories();
        LoadPlaces();
        OnPropertyChanged(nameof(ExploreTitle));
        OnPropertyChanged(nameof(PageHeading));
        OnPropertyChanged(nameof(PageSubtitle));
        OnPropertyChanged(nameof(AddFavoriteText));
        OnPropertyChanged(nameof(FavoriteSavedText));
        OnPropertyChanged(nameof(OkText));
    }

}
