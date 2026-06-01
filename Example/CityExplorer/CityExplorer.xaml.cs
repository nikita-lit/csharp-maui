namespace Example.CityExplorer;

using Example.CityExplorer.Services;
using Example.CityExplorer.ViewModels;
using Example.CityExplorer.Views;
using Microsoft.Extensions.DependencyInjection;

public partial class CityExplorerPage
{
    private readonly LocalizationService _localizationService;

    public CityExplorerPage()
    {
        InitializeComponent();
        _localizationService = App.Services?.GetService<LocalizationService>() ?? new LocalizationService();
        Title = "CityExplorer";

        Children.Add(CreatePage<ExplorePage, ExploreViewModel>());
        Children.Add(CreatePage<FavoritesPage, FavoritesViewModel>());
        Children.Add(CreatePage<SettingsPage, SettingsViewModel>());
    }

    private TPage CreatePage<TPage, TViewModel>()
        where TPage : Page
        where TViewModel : class
    {
        var services = App.Services;
        if (services is not null)
            return services.GetRequiredService<TPage>();

        var databaseService = new DatabaseService();
        object viewModel = typeof(TViewModel) == typeof(SettingsViewModel)
            ? new SettingsViewModel(_localizationService)
            : typeof(TViewModel) == typeof(FavoritesViewModel)
                ? new FavoritesViewModel(databaseService, _localizationService)
                : new ExploreViewModel(databaseService, _localizationService);

        return (TPage)Activator.CreateInstance(typeof(TPage), viewModel)!;
    }
}
