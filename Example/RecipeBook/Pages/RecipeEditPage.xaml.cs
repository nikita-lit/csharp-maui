namespace Example.RecipeBook.Pages;

public partial class RecipeEditPage
{
    private Recipe? _selectedRecipe;
    public event Action? OnRecipeSaved;

    public RecipeEditPage()
    {
        InitializeComponent();
    }

    public void Refresh(List<Category> categories)
    {
        EditRecipeCategoryPicker.ItemsSource = categories;
        ClearForm();
    }

    public void SetForEditing(Recipe recipe, List<Category> categories)
    {
        _selectedRecipe = recipe;
        FillForm(recipe);
        EditRecipeCategoryPicker.ItemsSource = categories;
        SetFormMode(isEditing: true);
    }

    private void FillForm(Recipe recipe)
    {
        EditRecipeNameEntry.Text = recipe.Name;
        EditRecipeDescriptionEditor.Text = recipe.Description;
        EditRecipeImageUrlEntry.Text = recipe.ImageUrl;
        EditRecipeImagePreview.Source = recipe.ImageUrl;
        var categories = EditRecipeCategoryPicker.ItemsSource?.OfType<Category>() ?? [];
        EditRecipeCategoryPicker.SelectedItem = categories.FirstOrDefault(c => c.Name == recipe.Category);
    }

    private void ClearForm()
    {
        EditRecipeNameEntry.Text = string.Empty;
        EditRecipeCategoryPicker.SelectedItem = null;
        EditRecipeDescriptionEditor.Text = string.Empty;
        EditRecipeImageUrlEntry.Text = string.Empty;
        EditRecipeImagePreview.Source = null;
        _selectedRecipe = null;
        SetFormMode(isEditing: false);
    }

    private void SetFormMode(bool isEditing)
    {
        AddRecipeButton.IsVisible = !isEditing;
        UpdateRecipeButton.IsVisible = isEditing;
        RecipeEditActionsGrid.IsVisible = isEditing;
        RecipeFormTitleLabel.Text = isEditing ? "Muuda retsepti" : "Uus retsept";
    }

    private void NewRecipeButton_Clicked(object sender, EventArgs e)
        => ClearForm();

    private void EditRecipeImageUrlEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var url = e.NewTextValue?.Trim() ?? string.Empty;
        EditRecipeImagePreview.Source = string.IsNullOrWhiteSpace(url) ? null : url;
    }

    private async void PickEditRecipeImageButton_Clicked(object sender, EventArgs e)
    {
        var imagePath = await PickAndSaveImageAsync();
        if (imagePath is not null)
        {
            EditRecipeImageUrlEntry.Text = imagePath;
            EditRecipeImagePreview.Source = imagePath;
        }
    }

    private async void AddRecipeButton_Clicked(object sender, EventArgs e)
    {
        if (!ValidateForm())
            return;

        RecipesManager.SaveRecipe(GetRecipeFromForm());
        SaveDone();
        await DisplayAlertAsync("Salvestatud", "Retsept lisati retseptiraamatusse.", "OK");
    }

    private async void UpdateRecipeButton_Clicked(object sender, EventArgs e)
    {
        if (_selectedRecipe is null || !ValidateForm())
            return;

        var recipes = RecipesManager.ReadRecipes();
        var recipeToUpdate = recipes.FirstOrDefault(r => RecipesManager.IsSameRecipe(r, _selectedRecipe));

        if (recipeToUpdate is null) 
            return;
        
        CopyRecipe(GetRecipeFromForm(), recipeToUpdate);
        RecipesManager.SaveAllRecipes(recipes);
        SaveDone();
        await DisplayAlertAsync("Salvestatud", "Retsepti muudatused salvestati.", "OK");
    }

    private async void DeleteSelectedRecipeButton_Clicked(object sender, EventArgs e)
    {
        if (_selectedRecipe is null)
        {
            await DisplayAlertAsync("Viga", "Palun vali retsept.", "OK");
            return;
        }

        var delete = await DisplayAlertAsync("Kinnitus", $"Kas soovid retsepti \"{_selectedRecipe.Name}\" kustutada?",
            "Jah", "Ei");
        
        if (!delete) 
            return;
        
        var recipes = RecipesManager.ReadRecipes();
        var recipeToDelete = recipes.FirstOrDefault(r => RecipesManager.IsSameRecipe(r, _selectedRecipe));

        if (recipeToDelete is null) 
            return;
        
        recipes.Remove(recipeToDelete);
        RecipesManager.SaveAllRecipes(recipes);
        SaveDone();
        await DisplayAlertAsync("Kustutatud", "Retsept kustutati.", "OK");
    }

    private void CancelRecipeButton_Clicked(object sender, EventArgs e)
        => ClearForm();

    private bool ValidateForm()
    {
        var (name, category, imageUrl) = (
            EditRecipeNameEntry.Text?.Trim() ?? string.Empty,
            (EditRecipeCategoryPicker.SelectedItem as Category)?.Name ?? string.Empty,
            EditRecipeImageUrlEntry.Text?.Trim() ?? string.Empty
        );

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(category) &&
            !string.IsNullOrWhiteSpace(imageUrl)) 
            return true;
        
        _ = DisplayAlertAsync("Viga", "Palun täida kõik väljad.", "OK");
        return false;
    }

    private Recipe GetRecipeFromForm() => new()
    {
        Name = EditRecipeNameEntry.Text!.Trim(),
        Category = (EditRecipeCategoryPicker.SelectedItem as Category)!.Name,
        Description = EditRecipeDescriptionEditor.Text?.Trim() ?? string.Empty,
        ImageUrl = EditRecipeImageUrlEntry.Text!.Trim()
    };

    private static void CopyRecipe(Recipe source, Recipe target)
    {
        target.Name = source.Name;
        target.Category = source.Category;
        target.Description = source.Description;
        target.ImageUrl = source.ImageUrl;
    }

    private void SaveDone()
    {
        ClearForm();
        OnRecipeSaved?.Invoke();
    }
    
    private static async Task<string?> PickAndSaveImageAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo is null)
            return null;

        var imageDirectory = Path.Combine(FileSystem.AppDataDirectory, "recipe_images");
        Directory.CreateDirectory(imageDirectory);

        var extension = Path.GetExtension(photo.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg";

        var imagePath = Path.Combine(imageDirectory, $"{Guid.NewGuid():N}{extension}");
        await using var sourceStream = await photo.OpenReadAsync();
        await using var targetStream = File.OpenWrite(imagePath);
        await sourceStream.CopyToAsync(targetStream);

        return imagePath;
    }
}
