using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Example.CityExplorer.Models;

public class Category : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Emoji { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public bool IsSelected
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
