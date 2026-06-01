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
        await _viewModel.LoadFavoritesAsync();
    }
}
