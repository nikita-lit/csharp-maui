using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls.Shapes;

namespace Example;

public partial class RGBPage : ContentPage
{
    private Slider _redSlider, _greenSlider, _blueSlider;
    private Label _redLabel, _greenLabel, _blueLabel;
    private Rectangle _boxView;
    private Rectangle _redBox, _greenBox, _blueBox;
    private Stepper _rgbStepper;
    private Label _title;

    public RGBPage()
    {
        var layout = new AbsoluteLayout();
        _title = new Label
        {
            Text = "RGB mudel",
            FontSize = 26,
            TextColor = Colors.SeaGreen
        };

        AbsoluteLayout.SetLayoutBounds(_title, new Rect(0.5, 0.05, -1, -1));
        AbsoluteLayout.SetLayoutFlags(_title, AbsoluteLayoutFlags.PositionProportional);

        // ----------------------------------------------
        double offsetY = 0.2;

        _redBox = CreateColorBox(0.2, 0.2 + offsetY);
        _greenBox = CreateColorBox(0.2, 0.3 + offsetY);
        _blueBox = CreateColorBox(0.2, 0.4 + offsetY);

        _redSlider = CreateSlider(0.2 + offsetY);
        _redLabel = CreateLabel("Punane = 00", 0.225 + offsetY);

        _greenSlider = CreateSlider(0.3 + offsetY);
        _greenLabel = CreateLabel("Roheline = 00", 0.325 + offsetY);

        _blueSlider = CreateSlider(0.4 + offsetY);
        _blueLabel = CreateLabel("Sinine = 00", 0.425 + offsetY);
    
        // ----------------------------------------------
        var rndBut = new Button { Text = "Juhuslik värv" };
        rndBut.Clicked += (sender, e) =>
        {
            _redSlider.Value = Random.Shared.Next(0, 256);
            _greenSlider.Value = Random.Shared.Next(0, 256);
            _blueSlider.Value = Random.Shared.Next(0, 256);
        };
        AbsoluteLayout.SetLayoutBounds(rndBut, new Rect(0.2, 0.525 + offsetY, 120, 50));
        AbsoluteLayout.SetLayoutFlags(rndBut, AbsoluteLayoutFlags.PositionProportional);

        // ----------------------------------------------
        _boxView = new Rectangle
        {
            Fill = Colors.Black,
            BackgroundColor = Colors.Transparent, 
            Stroke = Colors.Black,
            StrokeThickness = 4,
        };
        AbsoluteLayout.SetLayoutBounds(_boxView, new Rect(0.5, 0.15, 180, 180));
        AbsoluteLayout.SetLayoutFlags(_boxView, AbsoluteLayoutFlags.PositionProportional);

        // ----------------------------------------------
        _rgbStepper = new Stepper
        {
            Minimum = double.MinValue,
            Maximum = double.MaxValue,
            Increment = 10,
            Value = 0,
        };

        AbsoluteLayout.SetLayoutBounds(_rgbStepper, new Rect(0.7, 0.525 + offsetY, 150, -1));
        AbsoluteLayout.SetLayoutFlags(_rgbStepper, AbsoluteLayoutFlags.PositionProportional);

        _rgbStepper.ValueChanged += (s, e) =>
        {
            double delta = e.NewValue - e.OldValue;
            _redSlider.Value = Math.Clamp(_redSlider.Value + delta, 0, 255);
            _greenSlider.Value = Math.Clamp(_greenSlider.Value + delta, 0, 255);
            _blueSlider.Value = Math.Clamp(_blueSlider.Value + delta, 0, 255);
        };

        // ----------------------------------------------
        var stepLabel = new Label { Text = _rgbStepper.Increment.ToString() };
        AbsoluteLayout.SetLayoutBounds(stepLabel, new Rect(0.5, 0.625 + offsetY, -1, -1));
        AbsoluteLayout.SetLayoutFlags(stepLabel, AbsoluteLayoutFlags.PositionProportional);

        var stepSlider = new Slider
        {
            Value = _rgbStepper.Increment,
            Minimum = 0,
            Maximum = 100,
        };
        stepSlider.ValueChanged += (sender, e) => {
            double step = 1;
            var newStep = Math.Round(e.NewValue / step) * step;
            ((Slider)sender).Value = newStep;

            _rgbStepper.Increment = stepSlider.Value;
            stepLabel.Text = stepSlider.Value.ToString();
        };
        AbsoluteLayout.SetLayoutBounds(stepSlider, new Rect(0.8, 0.6 + offsetY, 200, 40));
        AbsoluteLayout.SetLayoutFlags(stepSlider, AbsoluteLayoutFlags.PositionProportional);

        _redSlider.Value = _rgbStepper.Value;
        _greenSlider.Value = _rgbStepper.Value;
        _blueSlider.Value = _rgbStepper.Value;

        // ----------------------------------------------
        layout.Children.Add(_title);

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
        layout.Children.Add(_rgbStepper);
        layout.Children.Add(stepLabel);
        layout.Children.Add(stepSlider);

        Content = layout;
    }

    private Rectangle CreateColorBox(double x, double y)
    {
        var box = new Rectangle
        {
            WidthRequest = 65,
            HeightRequest = 60,
            Fill = Colors.Black,
            BackgroundColor = Colors.Transparent,
            Stroke = Colors.Black,
            StrokeThickness = 4,
        };
        AbsoluteLayout.SetLayoutBounds(box, new Rect(x, y, 65, 60));
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
        AbsoluteLayout.SetLayoutBounds(s, new Rect(0.675, y, 220, 40));
        AbsoluteLayout.SetLayoutFlags(s, AbsoluteLayoutFlags.PositionProportional);
        return s;
    }

    private Label CreateLabel(string text, double y)
    {
        var l = new Label { Text = text };
        AbsoluteLayout.SetLayoutBounds(l, new Rect(0.45, y, -1, -1));
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

        _boxView.Fill = Color.FromRgb(((int)_redSlider.Value) / 255.0, ((int)_greenSlider.Value) / 255.0, ((int)_blueSlider.Value) / 255.0);
        _title.TextColor = Color.FromRgb(((int)_redSlider.Value) / 255.0, ((int)_greenSlider.Value) / 255.0, ((int)_blueSlider.Value) / 255.0);

        _redBox.Fill = Color.FromRgb(((int)_redSlider.Value) / 255.0, 0, 0);
        _greenBox.Fill = Color.FromRgb(0, ((int)_greenSlider.Value) / 255.0, 0);
        _blueBox.Fill = Color.FromRgb(0, 0, ((int)_blueSlider.Value) / 255.0);

        Dispatcher.Dispatch(async () =>
        {
            await _boxView.ScaleToAsync(1.1, 100);
            await _boxView.ScaleToAsync(1.0, 100);
        });
    }
}