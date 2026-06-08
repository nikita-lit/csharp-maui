using Example.CityExplorer.Models;
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

    private async void RemoveFavorite_Clicked( object sender, EventArgs e )
    {
        if ( sender is not Button button || button.BindingContext is not Place place )
            return;

        await _viewModel.RemoveFavorite( place );
    }
}