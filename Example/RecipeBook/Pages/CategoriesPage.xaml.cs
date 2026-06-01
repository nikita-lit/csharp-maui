using Example.RecipeBook.Models;
using Example.RecipeBook.Services;

namespace Example.RecipeBook.Pages;

public partial class CategoriesPage
{
    private Category? _selectedCategory;
    private Border? _selectedCategoryCard;

    public CategoriesPage()
    {
        InitializeComponent();
    }

    public void Refresh()
    {
        var categories = RecipesService.ReadCategories().OrderBy(c => c.Name).ToList();
        CategoriesCollectionView.ItemsSource = categories;
        CategoryCountLabel.Text = RecipesService.FormatCount(categories.Count, "kategooria", "kategooriat");
        _selectedCategoryCard = null;
        ClearForm();
    }

    private async void CategoryCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Category category || sender is not Border selectedCard)
            return;

        ResetSelectedCategoryCard();
        _selectedCategory = category;
        _selectedCategoryCard = selectedCard;
        HighlightSelectedCategoryCard(selectedCard);

        CategoryNameEntry.Text = category.Name;
        SetFormMode(isEditing: true);

        await selectedCard.ScaleTo(0.985, 70, Easing.CubicOut);
        await selectedCard.ScaleTo(1, 90, Easing.CubicOut);
    }

    // Lisa uus
    private void NewCategoryButton_Clicked(object sender, EventArgs e)
    {
        _selectedCategory = null;
        ClearForm();
        SetFormMode(isEditing: false, isVisible: true);
    }

    // Lisa
    private void AddCategoryButton_Clicked(object sender, EventArgs e)
    {
        var name = CategoryNameEntry.Text?.Trim() ?? string.Empty;
        if (!ValidateName(name))
            return;

        var categories = RecipesService.ReadCategories();
        if (CategoryExists(categories, name))
        {
            _ = DisplayAlertAsync("Viga", "Selline kategooria on juba olemas.", "OK");
            return;
        }

        categories.Add(new Category { Name = name });
        RecipesService.SaveCategories(categories);
        SaveDone();
    }

    // Muuda
    private void UpdateCategoryButton_Clicked(object sender, EventArgs e)
    {
        if (!HasSelectedCategory())
            return;

        var newName = CategoryNameEntry.Text?.Trim() ?? string.Empty;
        if (!ValidateName(newName))
            return;

        var categories = RecipesService.ReadCategories();
        if (CategoryExists(categories, newName, _selectedCategory!.Name))
        {
            _ = DisplayAlertAsync("Viga", "Selline kategooria on juba olemas.", "OK");
            return;
        }

        var recipes = RecipesService.ReadRecipes();
        foreach (var recipe in recipes.Where(r => r.Category == _selectedCategory.Name))
            recipe.Category = newName;

        var categoryToUpdate = categories.FirstOrDefault(c => c.Name == _selectedCategory.Name);
        if (categoryToUpdate is not null)
            categoryToUpdate.Name = newName;

        RecipesService.SaveCategories(categories);
        RecipesService.SaveAllRecipes(recipes);
        SaveDone();
    }

    // Kustuta
    private async void DeleteCategoryButton_Clicked(object sender, EventArgs e)
    {
        if (!HasSelectedCategory())
            return;

        var selected = _selectedCategory!;
        var recipeCount = RecipesService.ReadRecipes()
            .Count(r => r.Category.Equals(selected.Name, StringComparison.OrdinalIgnoreCase));

        if (recipeCount > 0)
        {
            await DisplayAlertAsync("Viga", 
                $"Kategooriat \"{selected.Name}\" ei saa kustutada, sest selles kategoorias on {recipeCount} retsept(i).", "OK");
            return;
        }

        var remove = await DisplayAlertAsync("Kinnitus",
            $"Kas soovid kategooria \"{selected.Name}\" kustutada?", "Jah", "Ei");
        if (!remove) 
            return;
        
        var categories = RecipesService.ReadCategories()
            .Where(c => !c.Name.Equals(selected.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        RecipesService.SaveCategories(categories);
        SaveDone();
    }

    // Tühista
    private void CancelCategoryButton_Clicked(object sender, EventArgs e)
    {
        _selectedCategory = null;
        ClearForm();
    }

    private void ClearForm()
    {
        CategoryNameEntry.Text = string.Empty;
        CategoriesCollectionView.SelectedItem = null;
        ResetSelectedCategoryCard();
        SetFormMode(isEditing: false, isVisible: false);
    }

    private void HighlightSelectedCategoryCard(Border card)
    {
        card.BackgroundColor = Color.FromArgb("#FFF4EF");
        card.Stroke = new SolidColorBrush(Color.FromArgb("#C0392B"));
        card.StrokeThickness = 2;
    }

    private void ResetSelectedCategoryCard()
    {
        if (_selectedCategoryCard is null)
            return;

        _selectedCategoryCard.BackgroundColor = Colors.White;
        _selectedCategoryCard.Stroke = new SolidColorBrush(Color.FromArgb("#E0E0E0"));
        _selectedCategoryCard.StrokeThickness = 1;
        _selectedCategoryCard.Scale = 1;
        _selectedCategoryCard = null;
    }

    private void SetFormMode(bool isEditing, bool isVisible = true)
    {
        CategoryFormLayout.IsVisible = isVisible;
        CategoryHintLabel.IsVisible = !isVisible;
        CategoryFormTitleLabel.Text = isEditing ? "Muuda kategooriat" : "Uus kategooria";
        AddCategoryButton.IsVisible = !isEditing;
        UpdateCategoryButton.IsVisible = isEditing;
        DeleteCategoryButton.IsVisible = isEditing;
        CancelCategoryButton.IsVisible = true;
        Grid.SetColumn(CancelCategoryButton, isEditing ? 2 : 1);
    }

    private bool HasSelectedCategory()
    {
        if (_selectedCategory is not null)
            return true;

        _ = DisplayAlertAsync("Viga", "Palun vali kategooria.", "OK");
        return false;
    }

    private bool ValidateName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            return true;

        _ = DisplayAlertAsync("Viga", "Palun sisesta kategooria nimi.", "OK");
        return false;
    }

    private static bool CategoryExists(IEnumerable<Category> categories, string name, string? except = null) =>
        categories.Any(c =>
            !c.Name.Equals(except, StringComparison.OrdinalIgnoreCase) &&
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    private void SaveDone()
    {
        Refresh();
        RecipeBookPage.Page.RefreshRecipes();
    }
}
