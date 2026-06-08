using Example.CityExplorer.Models;
using Example.CityExplorer.ViewModels;

namespace Example.CityExplorer.Views;

public partial class ExplorePage
{
    private readonly ExploreViewModel _viewModel;
    private bool _autoScroll;

    public ExplorePage()
    {
        InitializeComponent();
        _viewModel = new ExploreViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        await _viewModel.Load();
        StartAutoScroll();
    }

    protected override void OnDisappearing()
    {
        _autoScroll = false;
    }

    private void StartAutoScroll()
    {
        if ( _autoScroll )
            return;

        _autoScroll = true;
        Dispatcher.StartTimer( TimeSpan.FromSeconds( 4 ), () =>
        {
            if ( !_autoScroll || _viewModel.Places.Count == 0 )
                return false;

            AttractionsCarousel.Position = (AttractionsCarousel.Position + 1) % _viewModel.Places.Count;
            return true;
        } );
    }

    // Navigate to place details
    private async void PlaceCard_Tapped( object sender, TappedEventArgs e )
    {
        if ( e.Parameter is not Place place )
            return;

        await Navigation.PushAsync( new PlaceDetailPage( place ) );
    }

    // Select category
    private void CategoryCard_Tapped( object sender, TappedEventArgs e )
    {
        if ( e.Parameter is not Category category )
            return;

        _viewModel.SelectedCategory = category;
    }
}