using Example.CityExplorer.Models;
using SQLite;

namespace Example.CityExplorer.Services;

public class DatabaseService
{
    private static bool _sqliteInitialized;
    private readonly SQLiteAsyncConnection _database;
    private bool _isInitialized;

    public DatabaseService()
    {
        if (!_sqliteInitialized)
        {
            SQLitePCL.Batteries_V2.Init();
            _sqliteInitialized = true;
        }

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "city_explorer.db3");
        _database = new SQLiteAsyncConnection(databasePath);
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            await _database.CreateTableAsync<Place>();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CityExplorer DB init failed: {ex.Message}");
        }
    }

    public async Task AddFavoriteAsync(Place? place)
    {
        if (place is null)
            return;

        await InitializeAsync();

        try
        {
            place.IsFavorite = true;
            await _database.InsertOrReplaceAsync(place);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Add favorite failed: {ex.Message}");
        }
    }

    public async Task<List<Place>> GetFavoritesAsync()
    {
        await InitializeAsync();

        try
        {
            return await _database.Table<Place>().OrderBy(place => place.Name).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Read favorites failed: {ex.Message}");
            return [];
        }
    }

    public async Task RemoveFavoriteAsync(int id)
    {
        await InitializeAsync();

        try
        {
            await _database.DeleteAsync<Place>(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Remove favorite failed: {ex.Message}");
        }
    }
}
