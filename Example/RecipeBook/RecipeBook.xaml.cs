namespace Example.RecipeBook;

public partial class RecipeBookPage
{
    public RecipeBookPage()
    {
        InitializeComponent();
        InitializePages();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshAll();
    }

    private void InitializePages()
    {
        CategoriesPageTab.OnDataChanged += RefreshRecipes;
        RecipesPageTab.OnAddNewRecipe += OpenRecipeEdit;
        RecipeEditPageTab.OnRecipeSaved += RefreshAll;
        RecipesPageTab.SetCallbacks(EditRecipeFromDetails, RefreshAllAsync);
    }

    private void RefreshAll()
    {
        RefreshCategories();
        RefreshRecipes();
    }

    private Task RefreshAllAsync()
    {
        RefreshAll();
        return Task.CompletedTask;
    }

    private void RefreshCategories() => 
        CategoriesPageTab.Refresh();

    private void RefreshRecipes()
    {
        RecipesPageTab.Refresh();
        RecipeEditPageTab.Refresh(GetCategories());
    }

    private void OpenRecipeEdit()
    {
        RecipeEditPageTab.Refresh(GetCategories());
        CurrentPage = RecipeEditPageTab;
    }

    private Task EditRecipeFromDetails(Recipe recipe)
    {
        RecipeEditPageTab.SetForEditing(recipe, GetCategories());
        CurrentPage = RecipeEditPageTab;
        return Task.CompletedTask;
    }

    private static List<Category> GetCategories() =>
        RecipesManager.ReadCategories().OrderBy(c => c.Name).ToList();

    private async void BackToolbarItem_Clicked(object sender, EventArgs e) => 
        await Navigation.PopModalAsync();
}
