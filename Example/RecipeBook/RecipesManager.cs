using System.Text.Json;
using Example.RecipeBook.Models;

namespace Example.RecipeBook;

public static class RecipesManager
{
    private static readonly JsonSerializerOptions JsonOptions = new();
    private static readonly string RecipesFilePath = Path.Combine(FileSystem.AppDataDirectory, "recipes.json");
    private static readonly string CategoriesFilePath = Path.Combine(FileSystem.AppDataDirectory, "categories.json");

    public static void SaveRecipe(Recipe recipe)
    {
        var recipes = ReadRecipes();
        recipes.Add(recipe);
        SaveAllRecipes(recipes);
    }

    public static void SaveAllRecipes(List<Recipe> recipes) => 
        File.WriteAllText(RecipesFilePath, JsonSerializer.Serialize(recipes, JsonOptions));

    public static void SaveCategories(List<Category> categories) => 
        File.WriteAllText(CategoriesFilePath, JsonSerializer.Serialize(categories, JsonOptions));

    public static List<Recipe> ReadRecipes()
    {
        try
        {
            var json = File.ReadAllText(RecipesFilePath);
            return JsonSerializer.Deserialize<List<Recipe>>(json, JsonOptions) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public static List<Category> ReadCategories()
    {
        try
        {
            var json = File.ReadAllText(CategoriesFilePath);
            return JsonSerializer.Deserialize<List<Category>>(json, JsonOptions) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public static string FormatCount(int count, string singular, string plural) =>
        count == 1 ? $"1 {singular}" : $"{count} {plural}";

    public static bool IsSameRecipe(Recipe first, Recipe second) =>
        first.Name == second.Name &&
        first.Category == second.Category &&
        first.Description == second.Description &&
        first.ImageUrl == second.ImageUrl;
}
