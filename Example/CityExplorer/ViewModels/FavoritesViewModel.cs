using System.Collections.ObjectModel;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class FavoritesViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly LocalizationService _localizationService;

    public FavoritesViewModel(DatabaseService databaseService, LocalizationService localizationService)
    {
        _databaseService = databaseService;
        _localizationService = localizationService;
        Favorites = [];
        FavoriteGroups = [];
        _localizationService.LanguageChanged += (_, _) => _ = ReloadAsync();
    }

    public ObservableCollection<Place> Favorites { get; }
    public ObservableCollection<List<Place>> FavoriteGroups { get; }
    public string FavoritesTitle { get; private set; } = string.Empty;
    public string PageHeading { get; private set; } = string.Empty;
    public string EmptyText { get; private set; } = string.Empty;
    public string RemoveText { get; private set; } = string.Empty;
    public string SearchPlaceholder { get; private set; } = string.Empty;
    public string HintText { get; private set; } = string.Empty;

    public string SearchText
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            RefreshFavoriteGroups();
            OnPropertyChanged();
        }
    } = string.Empty;

    public void Load()
    {
        FavoritesTitle = _localizationService["FavoritesTitle"];
        PageHeading = _localizationService["FavoritesHeading"];
        EmptyText = _localizationService["FavoritesEmpty"];
        RemoveText = _localizationService["Remove"];
        SearchPlaceholder = _localizationService["FavoritesSearchPlaceholder"];
        HintText = _localizationService["FavoritesHint"];

        OnPropertyChanged(nameof(FavoritesTitle));
        OnPropertyChanged(nameof(PageHeading));
        OnPropertyChanged(nameof(EmptyText));
        OnPropertyChanged(nameof(RemoveText));
        OnPropertyChanged(nameof(SearchPlaceholder));
        OnPropertyChanged(nameof(HintText));
    }

    public async Task LoadFavoritesAsync()
    {
        var favorites = await _databaseService.GetFavoritesAsync();
        Favorites.Clear();

        foreach (var place in favorites)
            Favorites.Add(PlaceCatalog.LocalizePlace(place, _localizationService));

        RefreshFavoriteGroups();
    }

    public async Task RemoveFavoriteAsync(Place? place)
    {
        if (place is null)
            return;

        await _databaseService.RemoveFavoriteAsync(place.Id);
        await LoadFavoritesAsync();
    }

    private async Task ReloadAsync()
    {
        Load();
        await LoadFavoritesAsync();
    }

    private void RefreshFavoriteGroups()
    {
        var filteredFavorites = Favorites
            .Where(place => MatchesSearch(place))
            .OrderBy(place => place.Category)
            .ThenBy(place => place.Name)
            .ToList();

        var groupedFavorites = filteredFavorites
            .GroupBy(place => PlaceCatalog.GetCategoryTitle(place.Category, _localizationService))
            .ToList();

        FavoriteGroups.Clear();

        foreach (var group in groupedFavorites)
        {
            FavoriteGroups.Add(group.ToList());
        }
    }

    private bool MatchesSearch(Place place) =>
        string.IsNullOrWhiteSpace(SearchText) ||
        ContainsSearchText(place.Name) ||
        ContainsSearchText(place.Description) ||
        ContainsSearchText(place.Category);

    private bool ContainsSearchText(string value) =>
        value.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
}
