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
        Places = [];
        Categories = [];
        LocalizationService.LanguageChanged += ( _, _ ) => { _ = Load(); };
        _ = Load();
    }

    public ObservableCollection<Place> Places { get; }
    public ObservableCollection<Category> Categories { get; }
    public string ExploreTitle { get; private set; } = string.Empty;
    public string PageHeading { get; private set; } = string.Empty;
    public string PageSubtitle { get; private set; } = string.Empty;

    private Category? _selectedCategory;

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value) 
                return;
            
            _selectedCategory = value;
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
        var selectedKey = SelectedCategory?.Id ?? "Historical";
        Categories.Clear();

        var categories = await DatabaseService.GetAllCategories();
        foreach (var category in categories)
        {
            Categories.Add(new Category
            {
                Emoji = category.Emoji,
                Id = category.Id,
                Title = LocalizationService.Get(category.Title)
            });
        }
        
        _selectedCategory = Categories.FirstOrDefault(c => c.Id == selectedKey) ?? Categories.FirstOrDefault();
        UpdateCategorySelection();
        OnPropertyChanged(nameof(SelectedCategory));
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
            category.IsSelected = category.Id == SelectedCategory?.Id;
    }

    private void AddPlaces()
    {
        Places.Clear();
        var categoryId = SelectedCategory?.Id ?? "Historical";

        foreach ( var place in _allPlaces.Where( place => place.Category == categoryId ) )
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