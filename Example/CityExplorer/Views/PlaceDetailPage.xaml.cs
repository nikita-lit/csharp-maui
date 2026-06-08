using Example.CityExplorer.Models;
using Example.CityExplorer.Services;
using Example.CityExplorer.ViewModels;

namespace Example.CityExplorer.Views;

public partial class PlaceDetailPage
{
    private readonly PlaceDetailViewModel _viewModel;

    public PlaceDetailPage( Place place )
    {
        InitializeComponent();
        _viewModel = new PlaceDetailViewModel();
        BindingContext = _viewModel;

        var localizedPlace = new Place
        {
            Id = place.Id,
            Name = LocalizationService.Get( place.Name ),
            ShortDescription = LocalizationService.Get( place.ShortDescription ),
            Description = LocalizationService.Get( place.Description ),
            ImagePath = place.ImagePath,
            Category = LocalizationService.Get( place.Category ),
            IsFavorite = place.IsFavorite
        };

        _viewModel.Place = localizedPlace;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.Load();
    }
}