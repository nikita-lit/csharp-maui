using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Example;

public partial class LumememmPage : ContentPage
{
    private AbsoluteLayout _layout, _snowLayout;
    private Label _title, _opacityVLabel, _sizeLabel;
    private Picker _picker;
    private Image _bgImage;
    private Slider _opacitySlider;
    private Stepper _sizeStepper;
    private Frame _fader, _hat1, _hat2;
    private bool _isDay = true;
    private bool _isMelted = false;
    private bool _isMelting = false;
    private CancellationTokenSource? _danceCts;
    private Color _hatColor = Color.FromRgb(30, 30, 30);

    public LumememmPage()
    {
        _layout = new AbsoluteLayout();
        _title = new Label
        {
            Text = "Lumememm",
            FontSize = 32,
            TextColor = Colors.Red,
        };

        AbsoluteLayout.SetLayoutBounds(_title, new Rect(0.5, 0.03, -1, -1));
        AbsoluteLayout.SetLayoutFlags(_title, AbsoluteLayoutFlags.PositionProportional);

        _bgImage = new Image
        {
            Source = "snow_forest_day.png",
            Aspect = Aspect.AspectFill
        };

        AbsoluteLayout.SetLayoutBounds(_bgImage, new Rect(0.5, 0.01, 0.9, 0.64));
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
                "Peida",
                "Näita",
                "Muuda värvi",
                "Sulata",
                "Tantsi",
            },
            SelectedIndex = 0,
        };
        vsl.Children.Add(_picker);

        // buttons
        var hsl1 = new HorizontalStackLayout { Spacing = 8 };
        var butAct = new Button {
            Text = "Käivita tegevus",
            WidthRequest = 150,
        };
        butAct.Clicked += async (sender, e) => {
            StartAction();
            await butAct.ScaleToAsync(0.9, 250, Easing.SpringOut);
            await butAct.ScaleToAsync(1, 250, Easing.SpringOut);
        };

        var but = new Button {
            Text = "Öö",
            WidthRequest = 150,
        };
        but.Clicked += async (sender, e) =>
        {
            _isDay = !_isDay;
            if (_isDay)
            {
                _bgImage.Source = "snow_forest_day.png";
                but.Text = "Öö";
                _title.TextColor = Colors.Red;
            }
            else
            {
                _bgImage.Source = "snow_forest_night.png";
                but.Text = "Päev";
                _title.TextColor = Colors.Green;
            }

            _fader.IsVisible = !_isDay;

            await but.ScaleToAsync(0.9, 250, Easing.SpringOut);
            await but.ScaleToAsync(1, 250, Easing.SpringOut);
        };

        hsl1.Children.Add(butAct);
        hsl1.Children.Add(but);
        vsl.Children.Add(hsl1);

        // opacity
        var opacityLabel = new Label
        {
            Text = "Läbipaistmatus",
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Start,
        };
        vsl.Children.Add(opacityLabel);

        var hsl2 = new HorizontalStackLayout { Spacing = 8 };

        _opacityVLabel = new Label {
            Text = "1.00",
            WidthRequest = 40,
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

        // size stepper
        var hsl3 = new HorizontalStackLayout { Spacing = 10 };
        _sizeLabel = new Label { 
            Text = "Suurus: 1.0", 
            VerticalOptions = LayoutOptions.Center 
        };

        _sizeStepper = new Stepper
        {
            Minimum = 0.2,
            Maximum = 1.2,
            Increment = 0.1,
            Value = 1.0
        };

        _sizeStepper.ValueChanged += (sender, e) =>
        {
            _snowLayout.Scale = e.NewValue;
            _sizeLabel.Text = $"Suurus: {e.NewValue:F1}";
        };

        hsl3.Children.Add(_sizeLabel);
        hsl3.Children.Add(_sizeStepper);
        vsl.Children.Add(hsl3);

        // fader
        _fader = new Frame
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 150),
            BorderColor = Colors.Transparent,
            HasShadow = false,
            IsVisible = false,
            InputTransparent = true,
            CornerRadius = 0,
        };

        AbsoluteLayout.SetLayoutBounds(_fader, new Rect(0.475, 0, 0.925, 0.65));
        AbsoluteLayout.SetLayoutFlags(_fader, AbsoluteLayoutFlags.All);
        _layout.Children.Add(_fader);

        // ----------------------------------------------
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

        var armColor = Color.FromRgb(59, 33, 6);

        // arm 1
        _snowLayout.Children.Add(CreateRectangle(armColor, 0.25, 0.3, 70, 10));
        var rect1 = CreateRectangle(armColor, 0.14, 0.28, 40, 8);
        rect1.Rotation = 35;
        _snowLayout.Children.Add(rect1);

        var rect2 = CreateRectangle(armColor, 0.14, 0.32, 40, 8);
        rect2.Rotation = -35;
        _snowLayout.Children.Add(rect2);

        // arm 2
        _snowLayout.Children.Add(CreateRectangle(armColor, 0.75, 0.3, 70, 10));
        var rect1a = CreateRectangle(armColor, 0.84, 0.28, 40, 8);
        rect1a.Rotation = -35;
        _snowLayout.Children.Add(rect1a);

        var rect2a = CreateRectangle(armColor, 0.84, 0.32, 40, 8);
        rect2a.Rotation = 35;
        _snowLayout.Children.Add(rect2a);

        // body
        _snowLayout.Children.Add(CreateCircle(Colors.White, 0.5, 0.5, 170));
        _snowLayout.Children.Add(CreateCircle(Colors.White, 0.5, 0.25, 120));
        _snowLayout.Children.Add(CreateCircle(Colors.White, 0.5, 0.1, 90));

        // face
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.46, 0.14, 10));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.54, 0.14, 10));

        _snowLayout.Children.Add(CreateRectangle(Colors.Orange, 0.5, 0.17, 10, 8));

        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.5, 0.21, 6));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.48, 0.208, 6));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.46, 0.2, 6));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.52, 0.208, 6));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.54, 0.2, 6));

        // buttons
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.5, 0.27, 8));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.5, 0.31, 8));
        _snowLayout.Children.Add(CreateCircle(Colors.Black, 0.5, 0.35, 8));

        // scarf
        _snowLayout.Children.Add(CreateRectangle(Colors.Green, 0.5, 0.23, 100, 10));

        // hat
        _hat1 = CreateRectangle(_hatColor, 0.5, 0.08, 90, 20);
        _hat2 = CreateRectangle(_hatColor, 0.5, 0.01, 60, 50);

        _snowLayout.Children.Add(_hat1);
        _snowLayout.Children.Add(_hat2);

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

    private Frame CreateRectangle(Color bgColor, double x, double y, double w, double h)
    {
        var frame = new Frame
        {
            BackgroundColor = bgColor,
            HasShadow = false,
        };
        AbsoluteLayout.SetLayoutBounds(frame, new Rect(x, y, w, h));
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
        _sizeLabel.Scale = 1;
        _sizeStepper.Value = 1;

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
                _hatColor = Color.FromRgb(
                    Random.Shared.Next(180, 256),
                    Random.Shared.Next(180, 256),
                    Random.Shared.Next(180, 256));

                _hat1.BackgroundColor = _hatColor;
                _hat2.BackgroundColor = _hatColor;

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
            await _snowLayout.TranslateToAsync(40, -10, 500, Easing.SpringOut);

            _snowLayout.RotateToAsync(-15, 400, Easing.CubicIn);
            await _snowLayout.TranslateToAsync(-40, -10, 500, Easing.SpringOut);

            _snowLayout.RotateToAsync(0, 400, Easing.CubicIn);
            await _snowLayout.TranslateToAsync(0, 0, 500, Easing.SpringOut);

            await _snowLayout.RotateToAsync(360, 800, Easing.SpringOut);
            _snowLayout.Rotation = 0;
        }
    }
}