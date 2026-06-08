using System.Diagnostics;
using Example.CityExplorer.Models;
using SQLite;
using SQLitePCL;

namespace Example.CityExplorer.Services;

public static class DatabaseService
{
    private static SQLiteAsyncConnection _database = null!;
    
    private static Task? _initTask;

    public static Task Initialize()
    {
        return _initTask ??= InitializeDb();
    }
    
    private static async Task InitializeDb()
    {
        await ClearDatabase();
        
        try
        {
            Batteries_V2.Init();

            var databasePath = Path.Combine( FileSystem.AppDataDirectory, "city_explorer.db" );
            _database = new SQLiteAsyncConnection( databasePath );

            await _database.CreateTableAsync<Place>();
            await _database.CreateTableAsync<Category>();

            var categoryCount = await _database.Table<Category>().CountAsync();
            if (categoryCount == 0)
                await _database.InsertAllAsync(CreateDefaultCategoriesForDb());

            var placeCount = await _database.Table<Place>().CountAsync();
            if (placeCount == 0)
                await _database.InsertAllAsync(CreateDefaultPlacesForDb());

            Debug.WriteLine( "Database initialized successfully" );
        }
        catch ( Exception ex )
        {
            _initTask = null;
            Debug.WriteLine( $"CityExplorer DB init failed: {ex.Message}" );
            throw;
        }
    }
    
    public static async Task ClearDatabase()
    {
        _initTask = null;
        
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null!;
        }

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "city_explorer.db");
        if (File.Exists(databasePath))
            File.Delete(databasePath);
    }

    private static List<Category> CreateDefaultCategoriesForDb()
    {
        return
        [
            new Category { Emoji = "🏰", Key = "CategoryHistorical", Title = "CategoryHistorical" },
            new Category { Emoji = "🌳", Key = "CategoryParks", Title = "CategoryParks" },
            new Category { Emoji = "🍽️", Key = "CategoryRestaurants", Title = "CategoryRestaurants" }
        ];
    }

    private static List<Place> CreateDefaultPlacesForDb()
    {
        return
        [
            new Place
            {
                Id = 1, Category = "CategoryHistorical", ImagePath = "estonia.png", Name = "ToompeaName",
                ShortDescription = "ToompeaShort", Description = "ToompeaDescription"
            },
            new Place
            {
                Id = 2, Category = "CategoryHistorical", ImagePath = "germany.png", Name = "NevskyName",
                ShortDescription = "NevskyShort", Description = "NevskyDescription"
            },
            new Place
            {
                Id = 3, Category = "CategoryParks", ImagePath = "snow_forest_day.png", Name = "KadriorgName",
                ShortDescription = "KadriorgShort", Description = "KadriorgDescription"
            },
            new Place
            {
                Id = 4, Category = "CategoryParks", ImagePath = "snow_forest_night.png", Name = "JapaneseGardenName",
                ShortDescription = "JapaneseGardenShort", Description = "JapaneseGardenDescription"
            },
            new Place
            {
                Id = 5, Category = "CategoryRestaurants", ImagePath = "lasagne.jpg", Name = "OldeHansaName",
                ShortDescription = "OldeHansaShort", Description = "OldeHansaDescription"
            },
            new Place
            {
                Id = 6, Category = "CategoryRestaurants", ImagePath = "pizza.jpg", Name = "RataskaevuName",
                ShortDescription = "RataskaevuShort", Description = "RataskaevuDescription"
            }
        ];
    }

    public static async Task<List<Place>> GetAllPlaces()
    {
        await Initialize();
        try
        {
            return await _database.Table<Place>().ToListAsync();
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"Read all places failed: {ex.Message}" );
            return [];
        }
    }

    public static async Task<List<Category>> GetAllCategories()
    {
        await Initialize();
        try
        {
            return await _database.Table<Category>().ToListAsync();
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"Read all categories failed: {ex.Message}" );
            return [];
        }
    }

    public static async Task AddFavorite( Place place )
    {
        if ( place == null )
            return;

        await Initialize();
        try
        {
            var dbPlace = await _database.Table<Place>().FirstOrDefaultAsync( p => p.Id == place.Id );
            if ( dbPlace != null )
            {
                dbPlace.IsFavorite = true;
                await _database.UpdateAsync( dbPlace );
            }

            Debug.WriteLine( $"Added favorite: {place.Id}" );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"Add favorite failed: {ex.Message}" );
        }
    }

    public static async Task<List<Place>> GetFavorites()
    {
        await Initialize();
        try
        {
            return await _database.Table<Place>().Where( p => p.IsFavorite ).OrderBy( place => place.Name )
                .ToListAsync();
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"Read favorites failed: {ex.Message}" );
            return [];
        }
    }

    public static async Task RemoveFavorite( int id )
    {
        await Initialize();
        try
        {
            var dbPlace = await _database.Table<Place>().FirstOrDefaultAsync( p => p.Id == id );
            if ( dbPlace != null )
            {
                dbPlace.IsFavorite = false;
                await _database.UpdateAsync( dbPlace );
                Debug.WriteLine( $"Removed favorite: {id}" );
            }
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"Remove favorite failed: {ex.Message}" );
        }
    }

    public static async Task<bool> IsFavorite( int placeId )
    {
        await Initialize();
        try
        {
            var favorite = await _database.Table<Place>().FirstOrDefaultAsync( p => p.Id == placeId );
            return favorite != null && favorite.IsFavorite;
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"IsFavorite check failed: {ex.Message}" );
            return false;
        }
    }
}