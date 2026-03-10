using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Example;

public partial class LumememmPage : ContentPage
{
    private AbsoluteLayout _layout, _snowLayout;
    private Label _title;
    private Picker _picker;
    private Image _bgImage;
    private bool _isDay = true;
    private CancellationTokenSource? _danceCts;

    public LumememmPage()
    {
        _layout = new AbsoluteLayout();
        _title = new Label
        {
            Text = "Lumememm",
            FontSize = 32,
            TextColor = Colors.IndianRed,
        };

        AbsoluteLayout.SetLayoutBounds(_title, new Rect(0.5, 0.05, -1, -1));
        AbsoluteLayout.SetLayoutFlags(_title, AbsoluteLayoutFlags.PositionProportional);

        _bgImage = new Image
        {
            Source = "snow_forest_day.png",
            Aspect = Aspect.AspectFill
        };

        AbsoluteLayout.SetLayoutBounds(_bgImage, new Rect(0, 0, 1, 0.65));
        AbsoluteLayout.SetLayoutFlags(_bgImage, AbsoluteLayoutFlags.All);
        _layout.Children.Add(_bgImage);

        CreateSnowman();

        var vsl = new VerticalStackLayout { Spacing = 8, Padding = 10 };
        AbsoluteLayout.SetLayoutBounds(vsl, new Rect(0, 0.975, 1, 0.33));
        AbsoluteLayout.SetLayoutFlags(vsl, AbsoluteLayoutFlags.All);
        _layout.Children.Add(vsl);

        _picker = new Picker { 
            Title = "Tegevus",
            Items =
            {
                "Näita lumememm",
                "Peida lumememm",
                "Muuda värvi",
                "Sulata",
                "Tantsi",
            },
            SelectedIndex = 0,
        };
        _picker.SelectedIndexChanged += picker_SelectedIndexChanged;
        vsl.Children.Add(_picker);

        var but = new Button {
            Text = "Öö",
        };
        but.Clicked += (sender, e) =>
        {
            _isDay = !_isDay;
            if (_isDay)
            {
                _bgImage.Source = "snow_forest_day.png";
                but.Text = "Öö";
            }
            else
            {
                _bgImage.Source = "snow_forest_night.png";
                but.Text = "Päev";
            }
        };

        vsl.Children.Add(but);
        _layout.Children.Add(_title);

        Content = _layout;
    }

    void CreateSnowman()
    {
        _snowLayout = new AbsoluteLayout() {
            BackgroundColor = Colors.Transparent,
        };
        AbsoluteLayout.SetLayoutBounds(_snowLayout, new Rect(0, 0.3, 1, 0.65));
        AbsoluteLayout.SetLayoutFlags(_snowLayout, AbsoluteLayoutFlags.All);

        _snowLayout.Children.Add(CreateCircle(Colors.White, 0.5, 0.5, 200));
        _snowLayout.Children.Add(CreateCircle(Colors.White, 0.5, 0.3, 150));

        _layout.Children.Add(_snowLayout);
    }

    private Frame CreateCircle(Color bgColor, double x, double y, double size)
    {
        var frame = new Frame
        {
            BackgroundColor = bgColor,
            CornerRadius = 1000,
        };
        AbsoluteLayout.SetLayoutBounds(frame, new Rect(x, y, size, size));
        AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.PositionProportional);
        return frame;
    }

    private async void picker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _danceCts?.Cancel();
        _snowLayout.CancelAnimations();

        _snowLayout.Rotation = 0;
        _snowLayout.TranslationX = 0;
        _snowLayout.TranslationY = 0;
        _snowLayout.Scale = 1;

        switch (_picker.SelectedItem)
        {
            case "Näita lumememm":
                _snowLayout.IsVisible = true;
                break;
            case "Peida lumememm":
                _snowLayout.IsVisible = false;
                break;
            case "Muuda värvi":
                break;
            case "Sulata":
                _snowLayout.ScaleTo(0, 2500, Easing.SpringOut);
                await _snowLayout.FadeToAsync(0, 2500, Easing.SpringOut);
                break;
            case "Tantsi":
                _danceCts = new CancellationTokenSource();
                await StartDancing(_danceCts.Token);
                break;
        }
    }

    private async Task StartDancing(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _snowLayout.RotateToAsync(15, 400, Easing.CubicIn);
            await _snowLayout.TranslateToAsync(100, -10, 500, Easing.SpringOut);

            _snowLayout.RotateToAsync(-15, 400, Easing.CubicIn);
            await _snowLayout.TranslateToAsync(-100, -10, 500, Easing.SpringOut);

            _snowLayout.RotateToAsync(0, 400, Easing.CubicIn);
            await _snowLayout.TranslateToAsync(0, 0, 500, Easing.SpringOut);

            await _snowLayout.RotateToAsync(360, 800, Easing.SpringOut);
            _snowLayout.Rotation = 0;
        }
    }
}