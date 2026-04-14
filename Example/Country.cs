using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Example;

public class Country : INotifyPropertyChanged
{
    private string name = string.Empty;
    private string capital = string.Empty;
    private int population;
    private string flag = string.Empty;

    public string Name
    {
        get => name;
        set { name = value; OnPropertyChanged(); }
    }

    public string Capital
    {
        get => capital;
        set { capital = value; OnPropertyChanged(); }
    }

    public int Population
    {
        get => population;
        set { population = value; OnPropertyChanged(); }
    }

    public string Flag
    {
        get => flag;
        set { flag = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
