using Example.RecipeBook.Models;

namespace Example.RecipeBook;

public partial class RecipeBookPage
{
    public static RecipeBookPage Page;
    
    public RecipeBookPage()
    {
        InitializeComponent();
        Page = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshCategories();
        RefreshRecipes();
    }

    private void RefreshCategories() => 
        CategoriesPageTab.Refresh();

    public void RefreshRecipes()
    {
        RecipesPageTab.Refresh();
        RecipeEditPageTab.Refresh();
    }

    public void OpenRecipeEdit()
    {
        RecipeEditPageTab.Refresh();
        CurrentPage = RecipeEditPageTab;
    }

    public void EditRecipeFromDetails(Recipe recipe)
    {
        RecipeEditPageTab.SetForEditing(recipe);
        CurrentPage = RecipeEditPageTab;
    }

    private async void BackToolbarItem_Clicked(object sender, EventArgs e) => 
        await Navigation.PopModalAsync();
}
