using Example.RecipeBook.Models;

namespace Example.RecipeBook.Pages;

public partial class RecipeDetailsPage
{
    private readonly Recipe _recipe;

    public RecipeDetailsPage(Recipe recipe)
    {
        InitializeComponent();
        _recipe = recipe;
        ShowRecipe();
    }

    private void ShowRecipe()
    {
        RecipeImage.Source = _recipe.ImageUrl;
        RecipeName.Text = _recipe.Name;
        RecipeCategory.Text = _recipe.Category;
        RecipeDescription.Text = string.IsNullOrWhiteSpace(_recipe.Description) ? "Kirjeldus puudub." : _recipe.Description;
    }

    // Muuda
    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        RecipeBookPage.Page.EditRecipeFromDetails(_recipe);
    }

    // Kustuta
    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        var remove = await DisplayAlertAsync("Kinnitus", $"Kas soovid retsepti \"{_recipe.Name}\" kustutada?", "Jah", "Ei");
        if (!remove) 
            return;
        
        var recipes = RecipesManager.ReadRecipes();
        if (recipes.RemoveAll(x => RecipesManager.IsSameRecipe(x, _recipe)) == 0)
            return;
        
        RecipesManager.SaveAllRecipes(recipes);
        await Navigation.PopModalAsync();
        await DisplayAlertAsync("Kustutatud", "Retsept kustutati.", "OK");

        RecipeBookPage.Page.RefreshAll();
    }

    private async void CloseButton_Clicked(object sender, EventArgs e) 
        => await Navigation.PopModalAsync();
}
