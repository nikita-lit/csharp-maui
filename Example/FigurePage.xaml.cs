using Microsoft.Maui.Controls.Shapes;

namespace Example;

public partial class FigurePage : ContentPage
{
	private BoxView _boxView;
    private Ellipse _ellipse;
    private Polygon _polygon;
    List<string> _buttons = ["Tagasi", "Avaleht", "Edasi"];

    public FigurePage()
	{
        int r = Random.Shared.Next(255);
        int g = Random.Shared.Next(255);
        int b = Random.Shared.Next(255);

        _boxView = new BoxView()
        {
            Color = Color.FromRgb(r, g, b),
            WidthRequest = 200,
            HeightRequest = 200,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            CornerRadius = 30,
        };
        TapGestureRecognizer tap = new();
        _boxView.GestureRecognizers.Add(tap);
        tap.Tapped += (sender, e) =>
        {
            int r = Random.Shared.Next(255);
            int g = Random.Shared.Next(255);
            int b = Random.Shared.Next(255);
            _boxView.Color = Color.FromRgb(r, g, b);
            _boxView.WidthRequest = _boxView.Width + 20;
            _boxView.HeightRequest = _boxView.Height + 20;
            if (_boxView.WidthRequest > DeviceDisplay.MainDisplayInfo.Width/3)
            {
                _boxView.WidthRequest = 200;
                _boxView.HeightRequest = 200;
            }
        };

        _ellipse = new Ellipse
        {
            WidthRequest = 200,
            HeightRequest = 200,
            Fill = new SolidColorBrush(Color.FromRgb(b, g, r)),
            Stroke = Colors.BurlyWood,
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
        };
        _ellipse.GestureRecognizers.Add(tap);

        _polygon = new Polygon
        {
            Points = 
            {
                new Point(0, 200), 
                new Point(100, 0), 
                new Point(200, 200),
            },
            Fill = new SolidColorBrush(Color.FromRgb(g, b, r)),
            Stroke = Colors.BurlyWood,
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
        };

        double rot = 0;
        TapGestureRecognizer tap2 = new();
        _polygon.GestureRecognizers.Add(tap2);
        tap2.Tapped += (sender, e) =>
        {
            rot += 15;
            _polygon.RotateTo(rot);
        };

        var hsl = new HorizontalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
        for (int j = 0; j < 3; j++)
        {
            Button but = new Button
            {
                Text = _buttons[j],
                FontSize = 18,
                TextColor = Colors.Chocolate,
                BackgroundColor = Colors.Beige,
                CornerRadius = 10,
                HeightRequest = 40,
                ZIndex = j
            };

            but.Clicked += (sender, e) =>
            {
                Button but = (Button)sender;

                if (but.ZIndex == 0)
                {
                    Navigation.PushAsync(new TextPage());
                }
                else if (but.ZIndex == 1)
                {
                    Navigation.PopToRootAsync();
                }
                else
                {
                    Navigation.PushAsync(new ValgusfoorPage());
                }
            };

            hsl.Add(but);
        }

        var vsl = new VerticalStackLayout
        {
            Spacing = 15,
            Padding = 20,
            Children = { _boxView, _ellipse, _polygon, hsl },
            HorizontalOptions = LayoutOptions.Center
        };

        Content = vsl;
    }
}