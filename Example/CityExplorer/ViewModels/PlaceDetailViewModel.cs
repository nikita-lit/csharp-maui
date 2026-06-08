using System.Windows.Input;
using Example.CityExplorer.Models;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class PlaceDetailViewModel : BaseViewModel
{
    private string _addToFavoritesText = string.Empty;
    private Color _buttonBackgroundColor = Colors.Blue;
    private bool _isSaving;
    private Place _place = null!;
    private string _removeFromFavoritesText = string.Empty;
    private string _descriptionLabel = string.Empty;
    
    public PlaceDetailViewModel()
    {
        ToggleFavoriteCommand = new Command( async void () => await ToggleFavorite() );
    }

    public Place Place
    {
        get => _place;
        set
        {
            if ( _place == value )
                return;
            _place = value;
            OnPropertyChanged();
            UpdateFavoritesButton();
        }
    }

    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if ( _isSaving == value )
                return;
            _isSaving = value;
            OnPropertyChanged();
        }
    }

    public string AddToFavoritesText
    {
        get => _addToFavoritesText;
        private set
        {
            if ( _addToFavoritesText == value )
                return;
            _addToFavoritesText = value;
            OnPropertyChanged();
        }
    }

    public string RemoveFromFavoritesText
    {
        get => _removeFromFavoritesText;
        private set
        {
            if ( _removeFromFavoritesText == value )
                return;
            _removeFromFavoritesText = value;
            OnPropertyChanged();
        }
    }
    
    public string DescriptionLabel
    {
        get => _descriptionLabel;
        private set
        {
            if ( _descriptionLabel == value ) 
                return;
            _descriptionLabel = value;
            OnPropertyChanged();
        }
    }

    public Color ButtonBackgroundColor
    {
        get => _buttonBackgroundColor;
        private set
        {
            if ( _buttonBackgroundColor == value )
                return;
            _buttonBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public string FavoritesButtonText => Place?.IsFavorite == true ? RemoveFromFavoritesText : AddToFavoritesText;

    public ICommand ToggleFavoriteCommand { get; }

    public async Task Load()
    {
        AddToFavoritesText = LocalizationService.Get( "AddToFavorites" );
        RemoveFromFavoritesText = LocalizationService.Get( "RemoveFromFavorites" );
        DescriptionLabel = LocalizationService.Get( "DescriptionLabel" );
        
        OnPropertyChanged( nameof(FavoritesButtonText) );
        
        if ( Place != null )
        {
            Place.IsFavorite = await DatabaseService.IsFavorite( Place.Id );
            UpdateFavoritesButton();
        }
    }

    private void UpdateFavoritesButton()
    {
        if ( Place == null )
            return;

        ButtonBackgroundColor = Place.IsFavorite ? Colors.Red : Colors.Blue;
        OnPropertyChanged( nameof(FavoritesButtonText) );
    }

    private async Task ToggleFavorite()
    {
        if ( IsSaving || Place == null )
            return;

        IsSaving = true;

        try
        {
            if ( Place.IsFavorite )
            {
                await DatabaseService.RemoveFavorite( Place.Id );
                Place.IsFavorite = false;
            }
            else
            {
                await DatabaseService.AddFavorite( Place );
                Place.IsFavorite = true;
            }

            UpdateFavoritesButton();
        }
        finally
        {
            IsSaving = false;
        }
    }
}