using System.Collections.ObjectModel;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class FavoritesViewModel : BaseViewModel
{
    public FavoritesViewModel()
    {
        Favorites = [];
        FavoriteGroups = [];
        LocalizationService.LanguageChanged += ( _, _ ) => { _ = Reload(); };
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
            if ( field == value )
                return;

            field = value;
            RefreshFavoriteGroups();
            OnPropertyChanged();
        }
    } = string.Empty;

    public async Task Load()
    {
        FavoritesTitle = LocalizationService.Get( "FavoritesTitle" );
        PageHeading = LocalizationService.Get( "FavoritesHeading" );
        EmptyText = LocalizationService.Get( "FavoritesEmpty" );
        RemoveText = LocalizationService.Get( "Remove" );
        SearchPlaceholder = LocalizationService.Get( "FavoritesSearchPlaceholder" );
        HintText = LocalizationService.Get( "FavoritesHint" );

        OnPropertyChanged( nameof(FavoritesTitle) );
        OnPropertyChanged( nameof(PageHeading) );
        OnPropertyChanged( nameof(EmptyText) );
        OnPropertyChanged( nameof(RemoveText) );
        OnPropertyChanged( nameof(SearchPlaceholder) );
        OnPropertyChanged( nameof(HintText) );

        await LoadFavorites();
    }

    public async Task LoadFavorites()
    {
        var favorites = await DatabaseService.GetFavorites();
        Favorites.Clear();

        foreach ( var place in favorites )
        {
            var localized = new Place
            {
                Id = place.Id,
                Name = LocalizationService.Get( place.Name ),
                ShortDescription = LocalizationService.Get( place.ShortDescription ),
                Description = LocalizationService.Get( place.Description ),
                ImagePath = place.ImagePath,
                Category = place.Category,
                IsFavorite = place.IsFavorite
            };

            Favorites.Add( localized );
        }

        RefreshFavoriteGroups();
    }

    public async Task RemoveFavorite( Place place )
    {
        await DatabaseService.RemoveFavorite( place.Id );
        await LoadFavorites();
    }

    private async Task Reload()
    {
        Load();
        await LoadFavorites();
    }

    private void RefreshFavoriteGroups()
    {
        var filteredFavorites = Favorites
            .Where( MatchesSearch )
            .OrderBy( place => place.Category )
            .ThenBy( place => place.Name )
            .ToList();

        var groupedFavorites = filteredFavorites
            .GroupBy( place => place.Category )
            .ToList();

        FavoriteGroups.Clear();

        foreach ( var group in groupedFavorites )
            FavoriteGroups.Add( group.ToList() );
    }

    private bool MatchesSearch( Place place )
    {
        return string.IsNullOrWhiteSpace( SearchText ) ||
               ContainsSearchText( place.Name ) ||
               ContainsSearchText( place.Description ) ||
               ContainsSearchText( place.Category );
    }

    private bool ContainsSearchText( string value )
    {
        return value.Contains( SearchText, StringComparison.OrdinalIgnoreCase );
    }
}