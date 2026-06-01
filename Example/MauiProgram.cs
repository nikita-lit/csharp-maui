using Example.CityExplorer.Services;
using Example.CityExplorer.ViewModels;
using Example.CityExplorer.Views;
using Microsoft.Extensions.Logging;

namespace Example
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Comic Sans MS.ttf", "ComicSansMSRegular");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<LocalizationService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<ExploreViewModel>();
            builder.Services.AddTransient<FavoritesViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<ExplorePage>();
            builder.Services.AddTransient<FavoritesPage>();
            builder.Services.AddTransient<SettingsPage>();

            var app = builder.Build();
            App.Services = app.Services;
            return app;
        }
    }
}
