using System.Collections.ObjectModel;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class ExploreViewModel : BaseViewModel
{
    private readonly List<Place> _allPlaces;

    public ExploreViewModel()
    {
        _allPlaces = [];
        Places = new ObservableCollection<Place>();
        Categories = new ObservableCollection<Category>();
        LocalizationService.LanguageChanged += ( _, _ ) => { _ = Load(); };
        _ = Load();
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
            if ( field == value )
                return;

            field = value;
            UpdateCategorySelection();
            _ = LoadPlaces();
            OnPropertyChanged();
        }
    }

    public string AddFavoriteText { get; private set; } = string.Empty;
    public string FavoriteSavedText { get; private set; } = string.Empty;
    public string OkText { get; private set; } = string.Empty;

    public async Task Load()
    {
        RefreshLocalizedText();
        await LoadCategories();
        await LoadPlaces();
    }

    private void RefreshLocalizedText()
    {
        ExploreTitle = LocalizationService.Get( "ExploreTitle" );
        PageHeading = LocalizationService.Get( "ExploreHeading" );
        PageSubtitle = LocalizationService.Get( "ExploreSubtitle" );
        AddFavoriteText = LocalizationService.Get( "AddToFavorites" );
        FavoriteSavedText = LocalizationService.Get( "FavoriteSaved" );
        OkText = LocalizationService.Get( "Ok" );

        OnPropertyChanged( nameof(ExploreTitle) );
        OnPropertyChanged( nameof(PageHeading) );
        OnPropertyChanged( nameof(PageSubtitle) );
        OnPropertyChanged( nameof(AddFavoriteText) );
        OnPropertyChanged( nameof(FavoriteSavedText) );
        OnPropertyChanged( nameof(OkText) );
    }

    private async Task LoadCategories()
    {
        var selectedKey = SelectedCategory?.Key ?? "Historical";
        Categories.Clear();

        var dbCategories = await DatabaseService.GetAllCategories();
        foreach ( var category in dbCategories )
        {
            var localizedCategory = new Category
            {
                Emoji = category.Emoji,
                Key = category.Key,
                Title = LocalizationService.Get( category.Title )
            };
            Categories.Add( localizedCategory );
        }

        SelectedCategory = Categories.FirstOrDefault( category => category.Key == selectedKey ) ??
                           Categories.FirstOrDefault();
    }

    private async Task LoadPlaces()
    {
        _allPlaces.Clear();
        var places = await DatabaseService.GetAllPlaces();
        _allPlaces.AddRange( places );

        AddPlaces();
    }

    private void UpdateCategorySelection()
    {
        foreach ( var category in Categories )
            category.IsSelected = category.Key == SelectedCategory?.Key;
    }

    private void AddPlaces()
    {
        Places.Clear();
        var categoryKey = SelectedCategory?.Key ?? "Historical";

        foreach ( var place in _allPlaces.Where( place => place.Category == categoryKey ) )
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

            Places.Add( localized );
        }
    }
}