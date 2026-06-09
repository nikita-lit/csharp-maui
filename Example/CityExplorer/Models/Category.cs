using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace Example.CityExplorer.Models;

public class Category : INotifyPropertyChanged
{
    [PrimaryKey]
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;

    [Ignore]
    public bool IsSelected
    {
        get;
        set
        {
            if ( field == value )
                return;

            field = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged( [CallerMemberName] string propertyName = "" )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}