using Example.CityExplorer.Models;
using Example.CityExplorer.Services;
using Example.CityExplorer.ViewModels;

namespace Example.CityExplorer.Views;

public partial class FavoritesPage
{
    private readonly FavoritesViewModel _viewModel;

    public FavoritesPage()
    {
        InitializeComponent();
        _viewModel = new FavoritesViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        await _viewModel.Load();
    }

    private async void PlaceCard_Tapped( object sender, TappedEventArgs e )
    {
        if ( e.Parameter is not Place place )
            return;

        var dbPlaces = await DatabaseService.GetAllPlaces();
        var dbPlace = dbPlaces.First(p => p.Id == place.Id);
        
        await Navigation.PushAsync( new PlaceDetailPage( dbPlace ) );
    }
}