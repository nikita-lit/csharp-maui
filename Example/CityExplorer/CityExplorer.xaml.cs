using Example.CityExplorer.Views;
namespace Example.CityExplorer;

public partial class CityExplorerPage
{
    public CityExplorerPage()
    {
        InitializeComponent();
        Title = "CityExplorer";

        Children.Add( new ExplorePage() );
        Children.Add( new FavoritesPage() );
        Children.Add( new SettingsPage() );
    }
}