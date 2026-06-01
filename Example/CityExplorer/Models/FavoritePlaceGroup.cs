using System.Collections.ObjectModel;

namespace Example.CityExplorer.Models;

public class FavoritePlaceGroup : ObservableCollection<Place>
{
    public FavoritePlaceGroup(string key)
    {
        Key = key;
    }

    public string Key { get; }
}
