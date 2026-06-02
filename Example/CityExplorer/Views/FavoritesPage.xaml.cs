using Example.CityExplorer.Models;
using Example.CityExplorer.ViewModels;

namespace Example.CityExplorer.Views;

public partial class FavoritesPage
{
    private readonly FavoritesViewModel _viewModel;

    public FavoritesPage(FavoritesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
        await _viewModel.LoadFavoritesAsync();
    }

    private async void RemoveFavorite_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not Place place)
            return;

        await _viewModel.RemoveFavoriteAsync(place);
    }
}
