using Example.CityExplorer.Models;
using Example.CityExplorer.ViewModels;

namespace Example.CityExplorer.Views;

public partial class ExplorePage
{
    private readonly ExploreViewModel _viewModel;
    private bool _autoScroll;

    public ExplorePage(ExploreViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
        StartAutoScroll();
    }

    protected override void OnDisappearing()
    {
        _autoScroll = false;
        base.OnDisappearing();
    }

    private void StartAutoScroll()
    {
        if (_autoScroll)
            return;

        _autoScroll = true;
        Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
        {
            if (!_autoScroll || _viewModel.Places.Count == 0)
                return false;

            AttractionsCarousel.Position = (AttractionsCarousel.Position + 1) % _viewModel.Places.Count;
            return true;
        });
    }

    private async void PlaceCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Place place)
            return;

        var add = await DisplayAlertAsync(place.Name, place.Description, _viewModel.AddFavoriteText, _viewModel.OkText);
        if (!add)
            return;

        await _viewModel.AddToFavoritesAsync(place);
        await DisplayAlertAsync(place.Name, _viewModel.FavoriteSavedText, _viewModel.OkText);
    }

    private void CategoryCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Category category)
            return;

        _viewModel.SelectedCategory = category;
    }
}
