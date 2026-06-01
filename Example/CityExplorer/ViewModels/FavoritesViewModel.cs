using System.Collections.ObjectModel;
using System.Windows.Input;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class FavoritesViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly LocalizationService _localizationService;
    private readonly List<Place> _allFavorites = [];
    private string _searchText = string.Empty;

    public FavoritesViewModel(DatabaseService databaseService, LocalizationService localizationService)
    {
        _databaseService = databaseService;
        _localizationService = localizationService;
        Favorites = [];
        FavoriteGroups = [];
        LoadFavoritesCommand = new Command(async () => await LoadFavoritesAsync());
        RemoveFavoriteCommand = new Command<Place>(async place => await RemoveFavoriteAsync(place));
        _localizationService.LanguageChanged += (_, _) => RefreshLocalizedText();
    }

    public ObservableCollection<Place> Favorites { get; }
    public ObservableCollection<FavoritePlaceGroup> FavoriteGroups { get; }
    public ICommand LoadFavoritesCommand { get; }
    public ICommand RemoveFavoriteCommand { get; }

    public string FavoritesTitle => _localizationService["FavoritesTitle"];
    public string PageHeading => _localizationService["FavoritesHeading"];
    public string EmptyText => _localizationService["FavoritesEmpty"];
    public string RemoveText => _localizationService["Remove"];
    public string SearchPlaceholder => _localizationService["FavoritesSearchPlaceholder"];
    public string HintText => _localizationService["FavoritesHint"];

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                RefreshFavoriteGroups();
        }
    }

    public async Task LoadFavoritesAsync()
    {
        var favorites = await _databaseService.GetFavoritesAsync();
        Favorites.Clear();

        foreach (var place in favorites)
            Favorites.Add(PlaceCatalog.LocalizePlace(place, _localizationService));

        RefreshFavoriteGroups();
    }

    private async Task RemoveFavoriteAsync(Place? place)
    {
        if (place is null)
            return;

        await _databaseService.RemoveFavoriteAsync(place.Id);
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
            var favoriteGroup = new FavoritePlaceGroup(group.Key);
            foreach (var place in group)
                favoriteGroup.Add(place);

            FavoriteGroups.Add(favoriteGroup);
        }
    }

    private void RefreshLocalizedText()
    {
        OnPropertyChanged(nameof(FavoritesTitle));
        OnPropertyChanged(nameof(PageHeading));
        OnPropertyChanged(nameof(EmptyText));
        OnPropertyChanged(nameof(RemoveText));
        OnPropertyChanged(nameof(SearchPlaceholder));
        OnPropertyChanged(nameof(HintText));
        RefreshFavoriteGroups();
        LoadFavoritesCommand.Execute(null);
    }

    private bool MatchesSearch(Place place) =>
        string.IsNullOrWhiteSpace(SearchText) ||
        ContainsSearchText(place.Name) ||
        ContainsSearchText(place.Description) ||
        ContainsSearchText(place.Category);

    private bool ContainsSearchText(string value) =>
        value.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
}
