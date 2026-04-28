namespace Example;

public partial class FoodDetailPage : ContentPage
{
    public FoodDetailPage(FoodItem food)
    {
        InitializeComponent();
        
        BindingContext = food;
        Title = food.Name;

        AboutLabel.Text = LanguageService.Get("AboutDish");
        BackButton.Text = LanguageService.Get("BackToGallery");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (sender is VisualElement btn)
        {
            await btn.ScaleToAsync(0.95, 80, Easing.CubicIn);
            await btn.ScaleToAsync(1.0, 80, Easing.CubicOut);
        }

        await Navigation.PopAsync();
    }
}
