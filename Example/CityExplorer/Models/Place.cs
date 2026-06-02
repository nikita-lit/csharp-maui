using SQLite;

namespace Example.CityExplorer.Models;

public class Place
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;

    public string CategoryTitle { get; set; } = string.Empty;

    public bool IsFavorite { get; set; }
}
