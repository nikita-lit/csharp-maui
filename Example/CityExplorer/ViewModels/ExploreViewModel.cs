using System.Collections.ObjectModel;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class ExploreViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly LocalizationService _localizationService;
    private readonly List<Place> _allPlaces;

    public ExploreViewModel(DatabaseService databaseService, LocalizationService localizationService)
    {
        _databaseService = databaseService;
        _localizationService = localizationService;
        _allPlaces = PlaceCatalog.CreateSeedPlaces();

        Places = [];
        Categories = [];
        _localizationService.LanguageChanged += (_, _) => Load();
        Load();
    }

    public ObservableCollection<Place> Places { get; }
    public ObservableCollection<Category> Categories { get; }
    public string ExploreTitle { get; private set; } = string.Empty;
    public string PageHeading { get; private set; } = string.Empty;
    public string PageSubtitle { get; private set; } = string.Empty;

    public Category? SelectedCategory
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            UpdateCategorySelection();
            LoadPlaces();
            OnPropertyChanged();
        }
    }

    public string AddFavoriteText { get; private set; } = string.Empty;
    public string FavoriteSavedText { get; private set; } = string.Empty;
    public string OkText { get; private set; } = string.Empty;

    public void Load()
    {
        RefreshLocalizedText();
        LoadCategories();
    }

    private void LoadCategories()
    {
        var selectedKey = SelectedCategory?.Key ?? "Historical";
        Categories.Clear();
        Categories.Add(new Category { Emoji = "🏰", Key = "Historical", Title = _localizationService["CategoryHistorical"] });
        Categories.Add(new Category { Emoji = "🌳", Key = "Parks", Title = _localizationService["CategoryParks"] });
        Categories.Add(new Category { Emoji = "🍽️", Key = "Restaurants", Title = _localizationService["CategoryRestaurants"] });
        SelectedCategory = Categories.FirstOrDefault(category => category.Key == selectedKey) ?? Categories.FirstOrDefault();
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

    public async Task AddToFavoritesAsync(Place? place)
    {
        if (place is null)
            return;

        await _databaseService.AddFavoriteAsync(place);
    }

    private void RefreshLocalizedText()
    {
        ExploreTitle = _localizationService["ExploreTitle"];
        PageHeading = _localizationService["ExploreHeading"];
        PageSubtitle = _localizationService["ExploreSubtitle"];
        AddFavoriteText = _localizationService["AddToFavorites"];
        FavoriteSavedText = _localizationService["FavoriteSaved"];
        OkText = _localizationService["Ok"];

        OnPropertyChanged(nameof(ExploreTitle));
        OnPropertyChanged(nameof(PageHeading));
        OnPropertyChanged(nameof(PageSubtitle));
        OnPropertyChanged(nameof(AddFavoriteText));
        OnPropertyChanged(nameof(FavoriteSavedText));
        OnPropertyChanged(nameof(OkText));
    }

}
