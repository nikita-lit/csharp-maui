using Example.RecipeBook.Models;

namespace Example.RecipeBook.Pages;

public partial class RecipeEditPage
{
    private Recipe? _selectedRecipe;

    public RecipeEditPage()
    {
        InitializeComponent();
    }

    public void Refresh()
    {
        EditRecipeCategoryPicker.ItemsSource = RecipesManager.ReadCategories();
        ClearForm();
    }

    public void SetForEditing(Recipe recipe)
    {
        _selectedRecipe = recipe;
        FillForm(recipe);
        EditRecipeCategoryPicker.ItemsSource = RecipesManager.ReadCategories();
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

    // Lisa uus
    private void NewRecipeButton_Clicked(object sender, EventArgs e)
        => ClearForm();

    // Vali foto
    private async void PickEditRecipeImageButton_Clicked(object sender, EventArgs e)
    {
        var imagePath = await PickAndSaveImageAsync();
        if (imagePath is null) 
            return;
        
        EditRecipeImageUrlEntry.Text = imagePath;
        EditRecipeImagePreview.Source = imagePath;
    }

    // Lisa
    private async void AddRecipeButton_Clicked(object sender, EventArgs e)
    {
        if (!ValidateForm())
            return;

        RecipesManager.SaveRecipe(GetRecipeFromForm());
        SaveDone();
        await DisplayAlertAsync("Salvestatud", "Retsept lisati retseptiraamatusse.", "OK");
    }

    // Salvesta
    private async void UpdateRecipeButton_Clicked(object sender, EventArgs e)
    {
        if (_selectedRecipe is null || !ValidateForm())
            return;

        var recipes = RecipesManager.ReadRecipes();
        var recipeToUpdate = recipes.FirstOrDefault(r => RecipesManager.IsSameRecipe(r, _selectedRecipe));

        if (recipeToUpdate is null) 
            return;

        var recipeForm = GetRecipeFromForm();
        recipeToUpdate.Name = recipeForm.Name;
        recipeToUpdate.Category = recipeForm.Category;
        recipeToUpdate.Description = recipeForm.Description;
        recipeToUpdate.ImageUrl = recipeForm.ImageUrl;
        
        RecipesManager.SaveAllRecipes(recipes);
        SaveDone();
        await DisplayAlertAsync("Salvestatud", "Retsepti muudatused salvestati.", "OK");
    }

    // Kustuta
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

    // Tühista
    private void CancelRecipeButton_Clicked(object sender, EventArgs e)
        => ClearForm();

    private bool ValidateForm()
    {
        var name = EditRecipeNameEntry.Text.Trim();
        var category = (EditRecipeCategoryPicker.SelectedItem as Category)?.Name;
        var imageUrl = EditRecipeImageUrlEntry.Text.Trim();
        
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
    
    private void SaveDone()
    {
        ClearForm();
        RecipeBookPage.Page.RefreshAll();
    }
    
    private static async Task<string?> PickAndSaveImageAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo is null)
            return null;

        var imageDirectory = Path.Combine(FileSystem.AppDataDirectory, "recipe_images");
        Directory.CreateDirectory(imageDirectory);

        var extension = Path.GetExtension(photo.FileName);
        var imagePath = Path.Combine(imageDirectory, $"{Guid.NewGuid():N}{extension}");
        
        await using var sourceStream = await photo.OpenReadAsync();
        await using var targetStream = File.OpenWrite(imagePath);
        await sourceStream.CopyToAsync(targetStream);

        return imagePath;
    }
}
