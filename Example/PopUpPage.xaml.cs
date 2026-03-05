namespace Example;

public partial class PopUpPage : ContentPage
{
    public PopUpPage()
    {
        var alertButton = new Button
        {
            Text = "Teade",
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Center
        };
        alertButton.Clicked += OnAlertButtonClicked;

        var confirmButton = new Button
        {
            Text = "Jah või ei",
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Center
        };
        confirmButton.Clicked += OnConfirmButtonClicked;

        var optionsButton = new Button
        {
            Text = "Valik",
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Center
        };
        optionsButton.Clicked += OnOptionsButtonClicked;

        Content = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(0, 50, 0, 0),
            Children =
            {
                alertButton,
                confirmButton,
                optionsButton
            }
        };
    }

    private async void OnAlertButtonClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Teade", "Teil on uus teade", "OK");
    }

    private async void OnConfirmButtonClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlertAsync("Kinnitus", "Kas oled kindel?", "Olen kindel", "Ei ole kindel");

        await DisplayAlertAsync("Teade", "Teie valik on:" + (result ? "Jah" : "Ei"), "OK");
    }

    private async void OnOptionsButtonClicked(object sender, EventArgs e)
    {
        string choice = await DisplayActionSheetAsync("Mida teha?", "Loobu", "Kustutada", "Tantsida", "Laulda", "Joonestada");
    
        if (choice != null && choice != "Loobu")
        {
            await DisplayAlertAsync("Valik", "Sa valisid tegevuse: " + choice, "OK");
        }
    }
}