using System.Diagnostics;
using Example.CityExplorer.Models;
using SQLite;
using SQLitePCL;

namespace Example.CityExplorer.Services;

public static class DatabaseService
{
    private static SQLiteAsyncConnection _database = null!;
    private static bool _initialized = false;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public static async Task Initialize()
    {
        if ( _initialized )
            return;

        await Semaphore.WaitAsync();
        try
        {
            if ( _initialized )
                return;

            Batteries_V2.Init();

            var databasePath = Path.Combine( FileSystem.AppDataDirectory, "city_explorer.db" );
            _database = new SQLiteAsyncConnection( databasePath );

            await _database.CreateTableAsync<Place>();
            await _database.CreateTableAsync<Category>();

            var categoryCount = await _database.Table<Category>().CountAsync();
            if ( categoryCount == 0 )
            {
                var defaultCategories = CreateDefaultCategoriesForDb();
                await _database.InsertAllAsync( defaultCategories );
            }

            var placeCount = await _database.Table<Place>().CountAsync();
            if ( placeCount == 0 )
            {
                var defaultPlaces = CreateDefaultPlacesForDb();
                await _database.InsertAllAsync( defaultPlaces );
            }

            _initialized = true;
            Debug.WriteLine( "Database initialized successfully" );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"CityExplorer DB init failed: {ex.Message}" );
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static List<Category> CreateDefaultCategoriesForDb()
    {
        return
        [
            new Category { Emoji = "🏰", Key = "Historical", Title = "CategoryHistorical" },
            new Category { Emoji = "🌳", Key = "Parks", Title = "CategoryParks" },
            new Category { Emoji = "🍽️", Key = "Restaurants", Title = "CategoryRestaurants" }
        ];
    }

    private static List<Place> CreateDefaultPlacesForDb()
    {
        return
        [
            new Place
            {
                Id = 1, Category = "Historical", ImagePath = "estonia.png", Name = "ToompeaName",
                ShortDescription = "ToompeaShort", Description = "ToompeaDescription"
            },
            new Place
            {
                Id = 2, Category = "Historical", ImagePath = "germany.png", Name = "NevskyName",
                ShortDescription = "NevskyShort", Description = "NevskyDescription"
            },
            new Place
            {
                Id = 3, Category = "Parks", ImagePath = "snow_forest_day.png", Name = "KadriorgName",
                ShortDescription = "KadriorgShort", Description = "KadriorgDescription"
            },
            new Place
            {
                Id = 4, Category = "Parks", ImagePath = "snow_forest_night.png", Name = "JapaneseGardenName",
                ShortDescription = "JapaneseGardenShort", Description = "JapaneseGardenDescription"
            },
            new Place
            {
                Id = 5, Category = "Restaurants", ImagePath = "lasagne.jpg", Name = "OldeHansaName",
                ShortDescription = "OldeHansaShort", Description = "OldeHansaDescription"
            },
            new Place
            {
                Id = 6, Category = "Restaurants", ImagePath = "pizza.jpg", Name = "RataskaevuName",
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
            else
            {
                place.IsFavorite = true;
                await _database.InsertOrReplaceAsync( place );
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