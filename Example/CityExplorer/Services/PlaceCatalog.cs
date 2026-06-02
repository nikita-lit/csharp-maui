using Example.CityExplorer.Models;

namespace Example.CityExplorer.Services;

public static class PlaceCatalog
{
    public static List<Place> CreateSeedPlaces() =>
    [
        new() { Id = 1, Category = "Historical", ImagePath = "estonia.png" },
        new() { Id = 2, Category = "Historical", ImagePath = "germany.png" },
        new() { Id = 3, Category = "Parks", ImagePath = "snow_forest_day.png" },
        new() { Id = 4, Category = "Parks", ImagePath = "snow_forest_night.png" },
        new() { Id = 5, Category = "Restaurants", ImagePath = "lasagne.jpg" },
        new() { Id = 6, Category = "Restaurants", ImagePath = "pizza.jpg" }
    ];

    public static Place LocalizePlace(Place place, LocalizationService localizationService) => place.Id switch
    {
        1 => place.WithLocalized(localizationService, localizationService["ToompeaName"], localizationService["ToompeaShort"], localizationService["ToompeaDescription"]),
        2 => place.WithLocalized(localizationService, localizationService["NevskyName"], localizationService["NevskyShort"], localizationService["NevskyDescription"]),
        3 => place.WithLocalized(localizationService, localizationService["KadriorgName"], localizationService["KadriorgShort"], localizationService["KadriorgDescription"]),
        4 => place.WithLocalized(localizationService, localizationService["JapaneseGardenName"], localizationService["JapaneseGardenShort"], localizationService["JapaneseGardenDescription"]),
        5 => place.WithLocalized(localizationService, localizationService["OldeHansaName"], localizationService["OldeHansaShort"], localizationService["OldeHansaDescription"]),
        6 => place.WithLocalized(localizationService, localizationService["RataskaevuName"], localizationService["RataskaevuShort"], localizationService["RataskaevuDescription"]),
        _ => place
    };

    public static string GetCategoryTitle(string categoryKey, LocalizationService localizationService) => categoryKey switch
    {
        "Historical" => localizationService["CategoryHistorical"],
        "Parks" => localizationService["CategoryParks"],
        "Restaurants" => localizationService["CategoryRestaurants"],
        _ => categoryKey
    };

    private static Place WithLocalized(this Place place, LocalizationService localizationService, string name, string shortDescription, string description) =>
        new()
        {
            Id = place.Id,
            Name = name,
            ShortDescription = shortDescription,
            Description = description,
            ImagePath = place.ImagePath,
            Category = place.Category,
            CategoryTitle = GetCategoryTitle(place.Category, localizationService),
            IsFavorite = place.IsFavorite
        };
}
