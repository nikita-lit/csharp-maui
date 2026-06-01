using Example.RecipeBook.Models;
using Example.RecipeBook.Services;
using Microsoft.Maui.Controls.Shapes;

namespace Example.RecipeBook.Pages;

public partial class RecipesPage
{
    private string _searchText = string.Empty;
    private bool _isOpeningRecipe;

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
        var allRecipes = RecipesService.ReadRecipes();
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
        if (_isOpeningRecipe)
            return;

        var recipe = e.Parameter as Recipe;
        var selectedCard = sender as Border;
        if (recipe is null)
            return;

        _isOpeningRecipe = true;
        try
        {
            if (selectedCard is not null)
            {
                selectedCard.BackgroundColor = Color.FromArgb("#FFF4EF");
                selectedCard.Stroke = new SolidColorBrush(Color.FromArgb("#C0392B"));
                selectedCard.StrokeThickness = 2;
                await selectedCard.ScaleTo(0.985, 70, Easing.CubicOut);
                await selectedCard.ScaleTo(1, 90, Easing.CubicOut);
            }

            await Navigation.PushModalAsync(new RecipeDetailsPage(recipe));
        }
        finally
        {
            if (selectedCard is not null)
            {
                selectedCard.BackgroundColor = Colors.White;
                selectedCard.Stroke = new SolidColorBrush(Color.FromArgb("#E0E0E0"));
                selectedCard.StrokeThickness = 1;
                selectedCard.Scale = 1;
            }

            _isOpeningRecipe = false;
        }
    }

    private void OpenNewRecipeButton_Clicked(object sender, EventArgs e)
    {
        RecipeBookPage.Page.OpenRecipeEdit();
    }

    private bool ContainsSearchText(string value) => 
        value.Contains(_searchText, StringComparison.OrdinalIgnoreCase);

    private void UpdateRecipeCount(int visibleCount, int totalCount)
    {
        var text = RecipesService.FormatCount(totalCount, "retsept", "retsepti");
        if (!string.IsNullOrWhiteSpace(_searchText))
            text = $"{visibleCount} / {RecipesService.FormatCount(totalCount, "retsept", "retsepti")}";

        RecipeCountLabel.Text = text;
    }
}
