namespace Example.RecipeBook.Pages;

public partial class RecipeDetailsPage : ContentPage
{
    private readonly Recipe _recipe;
    private readonly Func<Recipe, Task>? _onEditClicked;
    private readonly Func<Task>? _onDeleteClicked;

    public RecipeDetailsPage(Recipe recipe, Func<Recipe, Task>? onEdit = null, Func<Task>? onDelete = null)
    {
        InitializeComponent();
        _recipe = recipe;
        _onEditClicked = onEdit;
        _onDeleteClicked = onDelete;
        ShowRecipe();
    }

    private void ShowRecipe()
    {
        RecipeImage.Source = _recipe.ImageUrl;
        RecipeName.Text = _recipe.Name;
        RecipeCategory.Text = _recipe.Category;
        RecipeDescription.Text = string.IsNullOrWhiteSpace(_recipe.Description) ? "Kirjeldus puudub." : _recipe.Description;
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        if (_onEditClicked is not null) await _onEditClicked(_recipe);
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        var remove = await DisplayAlertAsync("Kinnitus", $"Kas soovid retsepti \"{_recipe.Name}\" kustutada?", "Jah", "Ei");
        if (!remove) 
            return;
        
        var recipes = RecipesManager.ReadRecipes();
        if (recipes.RemoveAll(RecipesManager.IsSameRecipeTo(_recipe)) == 0)
            return;
        
        RecipesManager.SaveAllRecipes(recipes);
        await Navigation.PopModalAsync();
        await DisplayAlertAsync("Kustutatud", "Retsept kustutati.", "OK");
        
        if (_onDeleteClicked is not null) await _onDeleteClicked();
    }

    private async void CloseButton_Clicked(object sender, EventArgs e) 
        => await Navigation.PopModalAsync();
}
