using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Example;

public partial class LumememmPage : ContentPage
{
    private AbsoluteLayout _layout, _snowLayout;
    private Label _title, _opacityVLabel;
    private Picker _picker;
    private Image _bgImage;
    private Slider _opacitySlider;
    private bool _isDay = true;
    private bool _isMelted = false;
    private bool _isMelting = false;
    private CancellationTokenSource? _danceCts;

    public LumememmPage()
    {
        _layout = new AbsoluteLayout();
        _title = new Label
        {
            Text = "Lumememm",
            FontSize = 32,
            TextColor = Colors.Red,
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
                "Näita",
                "Peida",
                "Muuda värvi",
                "Sulata",
                "Tantsi",
            },
            SelectedIndex = 0,
        };
        vsl.Children.Add(_picker);

        var butAct = new Button {
            Text = "Käivita tegevus",
        };
        butAct.Clicked += async (sender, e) => {
            StartAction();
            await butAct.ScaleTo(0.9, 250, Easing.SpringOut);
            await butAct.ScaleTo(1, 250, Easing.SpringOut);
        };

        var but = new Button {
            Text = "Öö",
        };
        but.Clicked += async (sender, e) =>
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

            await but.ScaleTo(0.9, 250, Easing.SpringOut);
            await but.ScaleTo(1, 250, Easing.SpringOut);
        };

        vsl.Children.Add(butAct);
        vsl.Children.Add(but);

        var opacityLabel = new Label
        {
            Text = "Läbipaistmatus",
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Start,
        };
        vsl.Children.Add(opacityLabel);

        var hsl2 = new HorizontalStackLayout { 
            Spacing = 8, 
            Padding = 0, 
        };
        _opacityVLabel = new Label {
            Text = "1.00",
            WidthRequest = 30,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        _opacitySlider = new Slider
        {
            Value = 1,
            Minimum = 0,
            Maximum = 1,
            WidthRequest = 320
        };
        _opacitySlider.ValueChanged += (sender, e) => {
            if (_isMelting) return;
            _snowLayout.Opacity = e.NewValue;
            _opacityVLabel.Text = $"{e.NewValue:F2}";
        };

        hsl2.Children.Add(_opacityVLabel);
        hsl2.Children.Add(_opacitySlider);
        vsl.Children.Add(hsl2);

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
            HasShadow = false,
        };
        AbsoluteLayout.SetLayoutBounds(frame, new Rect(x, y, size, size));
        AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.PositionProportional);
        return frame;
    }

    private void SetOpacity(double opacity)
    {
        _opacitySlider.Value = opacity;
        _opacityVLabel.Text = $"{opacity:F2}";
    }

    private async void StartAction()
    {
        _danceCts?.Cancel();
        _snowLayout.CancelAnimations();

        _snowLayout.Rotation = 0;
        _snowLayout.TranslationX = 0;
        _snowLayout.TranslationY = 0;
        _snowLayout.Scale = 1;

        switch (_picker.SelectedItem)
        {
            case "Näita":
                _snowLayout.IsVisible = true;
                if (_opacitySlider.Value != 0)
                    _snowLayout.Opacity = _opacitySlider.Value;
                else
                {
                    _snowLayout.Opacity = 1;
                    SetOpacity(1);
                }

                _isMelted = false;
                break;
            case "Peida":
                _snowLayout.IsVisible = false;
                _snowLayout.Opacity = 0;
                SetOpacity(0);
                break;
            case "Muuda värvi":
                break;
            case "Sulata":
                _isMelting = true;

                if (_isMelted)
                {
                    _snowLayout.Scale = 0;
                    _snowLayout.ScaleTo(1, 2500, Easing.SpringOut);
                    SetOpacity(1);
                    await _snowLayout.FadeToAsync(1, 2500, Easing.SpringOut);
                }
                else
                {
                    _snowLayout.ScaleTo(0, 2500, Easing.SpringOut);
                    SetOpacity(1);
                    await _snowLayout.FadeToAsync(0, 2500, Easing.SpringOut);
                }

                _isMelting = false;
                _isMelted = !_isMelted;
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