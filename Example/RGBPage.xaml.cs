using Microsoft.Maui.Layouts;

namespace Example;

public partial class RGBPage : ContentPage
{
    private Slider _redSlider, _greenSlider, _blueSlider;
    private Label _redLabel, _greenLabel, _blueLabel;
    private BoxView _boxView;
    private BoxView _redBox, _greenBox, _blueBox;
    private Stepper _sizeStepper;

    public RGBPage()
    {
        var layout = new AbsoluteLayout();
        var title = new Label
        {
            Text = "RGB",
            FontSize = 26,
            TextColor = Colors.SeaGreen
        };

        AbsoluteLayout.SetLayoutBounds(title, new Rect(0.5, 0.05, -1, -1));
        AbsoluteLayout.SetLayoutFlags(title, AbsoluteLayoutFlags.PositionProportional);

        // ----------------------------------------------
        _redBox = CreateColorBox(0.25);
        _greenBox = CreateColorBox(0.5);
        _blueBox = CreateColorBox(0.75);

        _redSlider = CreateSlider(0.20);
        _redLabel = CreateLabel("Punane = 00", 0.25);

        _greenSlider = CreateSlider(0.30);
        _greenLabel = CreateLabel("Roheline = 00", 0.35);

        _blueSlider = CreateSlider(0.40);
        _blueLabel = CreateLabel("Sinine = 00", 0.45);
    
        // ----------------------------------------------
        var rndBut = new Button { Text = "Juhuslik värv" };
        rndBut.Clicked += (sender, e) =>
        {
            _redSlider.Value = Random.Shared.Next(0, 256);
            _greenSlider.Value = Random.Shared.Next(0, 256);
            _blueSlider.Value = Random.Shared.Next(0, 256);
        };
        AbsoluteLayout.SetLayoutBounds(rndBut, new Rect(0.5, 0.65, 200, 50));
        AbsoluteLayout.SetLayoutFlags(rndBut, AbsoluteLayoutFlags.PositionProportional);

        // ----------------------------------------------
        _boxView = new BoxView
        {
            Color = Colors.Black
        };
        AbsoluteLayout.SetLayoutBounds(_boxView, new Rect(0.5, 0.95, 180, 180));
        AbsoluteLayout.SetLayoutFlags(_boxView, AbsoluteLayoutFlags.PositionProportional);

        _sizeStepper = new Stepper
        {
            Minimum = 100,
            Maximum = 250,
            Increment = 10,
            Value = 180
        };
        _sizeStepper.ValueChanged += (sender, e) => 
        {
            AbsoluteLayout.SetLayoutBounds(_boxView, new Rect(0.5, 1.2, e.NewValue, e.NewValue));
        };

        AbsoluteLayout.SetLayoutBounds(_sizeStepper, new Rect(0.5, 1.4, -1, -1));
        AbsoluteLayout.SetLayoutFlags(_sizeStepper, AbsoluteLayoutFlags.PositionProportional);

        layout.Children.Add(title);

        layout.Children.Add(_redBox);
        layout.Children.Add(_greenBox);
        layout.Children.Add(_blueBox);

        layout.Children.Add(_redSlider);
        layout.Children.Add(_redLabel);
        layout.Children.Add(_greenSlider);
        layout.Children.Add(_greenLabel);
        layout.Children.Add(_blueSlider);
        layout.Children.Add(_blueLabel);

        layout.Children.Add(rndBut);

        layout.Children.Add(_boxView);
        layout.Children.Add(_sizeStepper);

        Content = layout;
    }

    private BoxView CreateColorBox(double x)
    {
        var box = new BoxView
        {
            WidthRequest = 65,
            HeightRequest = 60,
            CornerRadius = 15
        };
        AbsoluteLayout.SetLayoutBounds(box, new Rect(x, 0.12, 65, 60));
        AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.PositionProportional);
        return box;
    }

    private Slider CreateSlider(double y)
    {
        var s = new Slider
        {
            Minimum = 0,
            Maximum = 255
        };
        s.ValueChanged += OnSliderValueChanged;
        AbsoluteLayout.SetLayoutBounds(s, new Rect(0.5, y, 300, 40));
        AbsoluteLayout.SetLayoutFlags(s, AbsoluteLayoutFlags.PositionProportional);
        return s;
    }

    private Label CreateLabel(string text, double y)
    {
        var l = new Label { Text = text };
        AbsoluteLayout.SetLayoutBounds(l, new Rect(0.5, y, -1, -1));
        AbsoluteLayout.SetLayoutFlags(l, AbsoluteLayoutFlags.PositionProportional);
        return l;
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (sender == _redSlider)
            _redLabel.Text = String.Format("Punane = {0:X2}", (int)args.NewValue);
        else if (sender == _greenSlider)
            _greenLabel.Text = String.Format("Roheline = {0:X2}", (int)args.NewValue);
        else if (sender == _blueSlider)
            _blueLabel.Text = String.Format("Sinine = {0:X2}", (int)args.NewValue);

        _boxView.Color = Color.FromRgb((int)_redSlider.Value, (int)_greenSlider.Value, (int)_blueSlider.Value);

        _redBox.Color = Color.FromRgb((int)_redSlider.Value, 0, 0);
        _greenBox.Color = Color.FromRgb(0, (int)_greenSlider.Value, 0);
        _blueBox.Color = Color.FromRgb(0, 0, (int)_blueSlider.Value);
    }
}