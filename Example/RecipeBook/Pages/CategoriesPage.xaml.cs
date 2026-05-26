namespace Example.RecipeBook.Pages;

public partial class CategoriesPage
{
    private Category? _selectedCategory;
    public event Action? OnDataChanged;

    public CategoriesPage()
    {
        InitializeComponent();
    }

    public void Refresh()
    {
        var categories = RecipesManager.ReadCategories().OrderBy(c => c.Name).ToList();
        CategoriesCollectionView.ItemsSource = categories;
        CategoryCountLabel.Text = RecipesManager.FormatCount(categories.Count, "kategooria", "kategooriat");
        ClearForm();
    }

    private void CategoriesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedCategory = e.CurrentSelection.FirstOrDefault() as Category;
        CategoryNameEntry.Text = _selectedCategory?.Name ?? string.Empty;
        SetFormMode(isEditing: _selectedCategory is not null, isVisible: _selectedCategory is not null);
    }

    private void NewCategoryButton_Clicked(object sender, EventArgs e)
    {
        _selectedCategory = null;
        ClearForm();
        SetFormMode(isEditing: false, isVisible: true);
    }

    private void AddCategoryButton_Clicked(object sender, EventArgs e)
    {
        var name = CategoryNameEntry.Text?.Trim() ?? string.Empty;
        if (!ValidateName(name))
            return;

        var categories = RecipesManager.ReadCategories();
        if (CategoryExists(categories, name))
        {
            _ = DisplayAlertAsync("Viga", "Selline kategooria on juba olemas.", "OK");
            return;
        }

        categories.Add(new Category { Name = name });
        RecipesManager.SaveCategories(categories);
        SaveDone();
    }

    private void UpdateCategoryButton_Clicked(object sender, EventArgs e)
    {
        if (!HasSelectedCategory())
            return;

        var newName = CategoryNameEntry.Text?.Trim() ?? string.Empty;
        if (!ValidateName(newName))
            return;

        var categories = RecipesManager.ReadCategories();
        if (CategoryExists(categories, newName, _selectedCategory!.Name))
        {
            _ = DisplayAlertAsync("Viga", "Selline kategooria on juba olemas.", "OK");
            return;
        }

        var recipes = RecipesManager.ReadRecipes();
        foreach (var recipe in recipes.Where(r => r.Category == _selectedCategory.Name))
            recipe.Category = newName;

        var categoryToUpdate = categories.FirstOrDefault(c => c.Name == _selectedCategory.Name);
        if (categoryToUpdate is not null)
            categoryToUpdate.Name = newName;

        RecipesManager.SaveCategories(categories);
        RecipesManager.SaveAllRecipes(recipes);
        SaveDone();
    }

    private async void DeleteCategoryButton_Clicked(object sender, EventArgs e)
    {
        if (!HasSelectedCategory())
            return;

        var selected = _selectedCategory!;
        var recipeCount = RecipesManager.ReadRecipes()
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
        
        var categories = RecipesManager.ReadCategories()
            .Where(c => !c.Name.Equals(selected.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        RecipesManager.SaveCategories(categories);
        SaveDone();
    }

    private void CancelCategoryButton_Clicked(object sender, EventArgs e)
    {
        _selectedCategory = null;
        ClearForm();
    }

    private void ClearForm()
    {
        CategoryNameEntry.Text = string.Empty;
        CategoriesCollectionView.SelectedItem = null;
        SetFormMode(isEditing: false, isVisible: false);
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
        OnDataChanged?.Invoke();
    }
}
