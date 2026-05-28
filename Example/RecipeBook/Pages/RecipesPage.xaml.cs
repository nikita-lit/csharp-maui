using Example.RecipeBook.Models;

namespace Example.RecipeBook.Pages;

public partial class RecipesPage
{
    private string _searchText = string.Empty;

    public RecipesPage()
    {
        InitializeComponent();
    }

    public void Refresh()
    {
        RefreshRecipeList();
    }

    private void RefreshRecipeList()
    {
        var allRecipes = RecipesManager.ReadRecipes();
        IEnumerable<Recipe> recipes = allRecipes.OrderBy(r => r.Category).ThenBy(r => r.Name);

        if (!string.IsNullOrWhiteSpace(_searchText))
            recipes = recipes.Where(r => 
                ContainsSearchText(r.Name) || 
                ContainsSearchText(r.Category) || 
                ContainsSearchText(r.Description));

        var filteredRecipes = recipes.ToList();
        var groups = filteredRecipes
            .GroupBy(r => r.Category)
            .ToList();

        RecipesCollectionView.ItemsSource = groups;
        UpdateRecipeCount(filteredRecipes.Count, allRecipes.Count);
    }

    private void RecipeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = e.NewTextValue?.Trim() ?? string.Empty;
        RefreshRecipeList();
    }

    private async void RecipeCard_Tapped(object sender, TappedEventArgs e)
    {
        var recipe = e.Parameter as Recipe;
        if (recipe is null && sender is TapGestureRecognizer tapGesture)
            recipe = tapGesture.CommandParameter as Recipe;

        if (recipe is null) 
            return;
        
        var detailsPage = new RecipeDetailsPage(recipe);
        await Navigation.PushModalAsync(detailsPage);
    }

    private void OpenNewRecipeButton_Clicked(object sender, EventArgs e)
    {
        RecipeBookPage.Page.OpenRecipeEdit();
    }

    private bool ContainsSearchText(string value) => 
        value.Contains(_searchText, StringComparison.OrdinalIgnoreCase);

    private void UpdateRecipeCount(int visibleCount, int totalCount)
    {
        var text = RecipesManager.FormatCount(totalCount, "retsept", "retsepti");
        if (!string.IsNullOrWhiteSpace(_searchText))
            text = $"{visibleCount} / {RecipesManager.FormatCount(totalCount, "retsept", "retsepti")}";

        RecipeCountLabel.Text = text;
    }
}
