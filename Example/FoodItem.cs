using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Example;

public class FoodItem : INotifyPropertyChanged
{
    public string Name 
    { 
        get => field; 
        set { field = value; OnPropertyChanged(); } 
    } = string.Empty;    
    
    public string Description 
    { 
        get => field; 
        set { field = value; OnPropertyChanged(); } 
    } = string.Empty;    

    public string DetailInfo 
    { 
        get => field; 
        set { field = value; OnPropertyChanged(); } 
    } = string.Empty;

    public string TapHint 
    { 
        get => field; 
        set { field = value; OnPropertyChanged(); } 
    } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
