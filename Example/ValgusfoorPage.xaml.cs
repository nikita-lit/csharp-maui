using Microsoft.Maui.Controls.Shapes;

namespace Example;

public partial class ValgusfoorPage : ContentPage
{
    List<string> _buttons = ["Tagasi", "Avaleht", "Edasi"];

    public ValgusfoorPage()
	{
        var ellipse1 = new Ellipse
        {
            WidthRequest = 200,
            HeightRequest = 200,
            Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
            Stroke = Colors.BurlyWood,
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
        };

        var ellipse2 = new Ellipse
        {
            WidthRequest = 200,
            HeightRequest = 200,
            Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0)),
            Stroke = Colors.BurlyWood,
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
        };

        var ellipse3 = new Ellipse
        {
            WidthRequest = 200,
            HeightRequest = 200,
            Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
            Stroke = Colors.BurlyWood,
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
        };


        var hsl = new HorizontalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
        for (int j = 0; j < 3; j++)
        {
            Button but = new Button
            {
                Text = _buttons[j],
                FontSize = 18,
                TextColor = Colors.White,
                BackgroundColor = Colors.LightGray,
                CornerRadius = 10,
                HeightRequest = 40,
                ZIndex = j
            };

            but.Clicked += (sender, e) =>
            {
                Button but = (Button)sender;

                if (but.ZIndex == 0)
                {
                    Navigation.PushAsync(new FigurePage());
                }
                else if (but.ZIndex == 1)
                {
                    Navigation.PopToRootAsync();
                }
                else
                {
                    //Navigation.PushAsync(new FigurePage());
                }
            };


            hsl.Add(but);
        }

        var vsl = new VerticalStackLayout
        {
            Spacing = 15,
            Padding = 20,
            Children = { ellipse1, ellipse2, ellipse3, hsl },
            HorizontalOptions = LayoutOptions.Center
        };

        Content = vsl;
    }
}