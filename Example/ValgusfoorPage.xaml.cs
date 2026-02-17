using Microsoft.Maui.Controls.Shapes;
using System;

namespace Example;

public partial class ValgusfoorPage : ContentPage
{
    public enum Mode
    {
        Manual,
        Auto,
        Night,
    }

    List<string> _buttons = ["Tagasi", "Avaleht", "Edasi"];
    List<string> _modes = ["Käsitsijuhtimine", "Automaatrežiim", "Öörežiim"];

    private Label _label;
    private Ellipse _ellipse1;
    private Ellipse _ellipse2;
    private Ellipse _ellipse3;
    private Button _turnOn;
    private Button _turnOff;
    private VerticalStackLayout _vslMode;

    private Mode _currentMode = Mode.Manual;
    private Mode _lastMode = Mode.Manual;
    private CancellationTokenSource? _modeCts;

    private bool _enabled = false;
    public bool Enabled 
    { 
        get { return _enabled; }
        private set
        {
            _enabled = value;
            if (_enabled)
            {
                _currentMode = _lastMode;
                StartMode();
                UpdateModeButtons();
            }
            else
            {
                StopMode();

                _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));

                _label.Text = "Lülita esmalt foor sisse";
            }
        }
    }

    public ValgusfoorPage()
	{
        _label = new Label
        {
            FontSize = 36,
            TextColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
        };
         
        var ellipseContainer = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            ColumnSpacing = 20,
        };

        _ellipse1 = new Ellipse
        {
            WidthRequest = 150,
            HeightRequest = 150,
            Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
            Stroke = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
        };
        _ellipse1.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                if (Enabled && _currentMode == Mode.Manual)
                {
                    await Task.WhenAll(
                        _ellipse1.ScaleToAsync(0.9, 100),
                        _ellipse1.FadeToAsync(0.7, 100)
                    );
                    await Task.WhenAll(
                        _ellipse1.ScaleToAsync(1.0, 100),
                        _ellipse1.FadeToAsync(1.0, 100)
                    );

                    _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 100));
                    _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));

                    _label.Text = "Seisa";
                }
            })
        });

        _ellipse2 = new Ellipse
        {
            WidthRequest = 150,
            HeightRequest = 150,
            Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
            Stroke = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
        };
        _ellipse2.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                if (Enabled && _currentMode == Mode.Manual)
                {
                    await Task.WhenAll(
                        _ellipse2.ScaleToAsync(0.9, 100),
                        _ellipse2.FadeToAsync(0.7, 100)
                    );
                    await Task.WhenAll(
                        _ellipse2.ScaleToAsync(1.0, 100),
                        _ellipse2.FadeToAsync(1.0, 100)
                    );

                    _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
                    _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));

                    _label.Text = "Valmista";
                }
            })
        });

        _ellipse3 = new Ellipse
        {
            WidthRequest = 150,
            HeightRequest = 150,
            Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
            Stroke = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
            StrokeThickness = 5,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
        };
        _ellipse3.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                if (Enabled && _currentMode == Mode.Manual)
                {
                    await Task.WhenAll(
                        _ellipse3.ScaleToAsync(0.9, 100),
                        _ellipse3.FadeToAsync(0.7, 100)
                    );
                    await Task.WhenAll(
                        _ellipse3.ScaleToAsync(1.0, 100),
                        _ellipse3.FadeToAsync(1.0, 100)
                    );

                    _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
                    _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 100));
                    _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));

                    _label.Text = "Sõida";
                }
            })
        });

        ellipseContainer.Add(_ellipse1, 0, 0);
        ellipseContainer.Add(_ellipse2, 0, 1);
        ellipseContainer.Add(_ellipse3, 0, 2);

        var butHsl = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center };
        _turnOn = new Button
        {
            Text = "Sisse",
            FontSize = 18,
            TextColor = Colors.White,
            BackgroundColor = Colors.Black,
            CornerRadius = 10,
            HeightRequest = 40,
        };
        _turnOn.Clicked += (sender, e) => {
            if (Enabled) 
                return;

            Enabled = true;
            _turnOn.BackgroundColor = Colors.LightGray;
            _turnOn.TextColor = Colors.Black;
            _turnOff.BackgroundColor = Colors.Black;
            _turnOff.TextColor = Colors.White;
        };

        _turnOff = new Button
        {
            Text = "Välja",
            FontSize = 18,
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightGray,
            CornerRadius = 10,
            HeightRequest = 40,
        };
        _turnOff.Clicked += (sender, e) => {
            if (!Enabled)
                return;

            Enabled = false;
            _turnOff.BackgroundColor = Colors.LightGray;
            _turnOff.TextColor = Colors.Black;
            _turnOn.BackgroundColor = Colors.Black;
            _turnOn.TextColor = Colors.White;
        };

        butHsl.Add(_turnOn);
        butHsl.Add(_turnOff);

        _vslMode = new VerticalStackLayout
        {
            Spacing = 15,
            Padding = 20,
            HorizontalOptions = LayoutOptions.Center
        };

        foreach (var (index, mode) in _modes.Index())
        {
            var but = new Button
            {
                Text = mode,
                FontSize = 18,
                TextColor = Colors.White,
                BackgroundColor = Colors.Black,
                CornerRadius = 10,
                HeightRequest = 40,
                ZIndex = index,
            };
            but.Clicked += (s, e) =>
            {
                if (_currentMode == (Mode)index)
                    return;

                _currentMode = (Mode)index;
                _lastMode = _currentMode;
                if (Enabled)
                    StartMode();

                UpdateModeButtons();
            };

            _vslMode.Add(but);
        }

        butHsl.Add(_vslMode);
        UpdateModeButtons();

        var hsl = new HorizontalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
        for (int j = 0; j < 3; j++)
        {
            Button but = new Button
            {
                Text = _buttons[j],
                FontSize = 18,
                TextColor = Colors.Black,
                BackgroundColor = Colors.LightGrey,
                CornerRadius = 10,
                HeightRequest = 40,
                ZIndex = j
            };

            but.Clicked += (sender, e) =>
            {
                Button but = (Button)sender;

                if (but.ZIndex == 0)
                    Navigation.PushAsync(new FigurePage());
                else if (but.ZIndex == 1)
                    Navigation.PopToRootAsync();
                //else
                    //Navigation.PushAsync(new FigurePage());
            };


            hsl.Add(but);
        }

        var vsl = new VerticalStackLayout
        {
            Spacing = 15,
            Padding = 20,
            Children = { _label, ellipseContainer, butHsl, hsl },
            HorizontalOptions = LayoutOptions.Center
        };

        Content = vsl;
        Enabled = false;
    }

    private void StartMode()
    {
        _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
        _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 100));
        _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));

        if (_currentMode == Mode.Manual)
        {
            _label.Text = "Vali valgus";
            return;
        }

        _modeCts?.Cancel();
        _modeCts = new CancellationTokenSource();
        var token = _modeCts.Token;

        Task.Run(async () =>
        {
            int index = 2;
            int delay = (_currentMode == Mode.Night) ? 500 : 4000;
            while (!token.IsCancellationRequested)
            {
                Dispatcher.Dispatch(async () =>
                {
                    if (_currentMode == Mode.Auto)
                    {
                        switch (index)
                        {
                            case 0:
                                _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                                _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 100));
                                _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));
                                _label.Text = "Seisa";
                                delay = 2000;
                                break;
                            case 1:
                                _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
                                _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                                _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));
                                _label.Text = "Valmista";
                                delay = 4000;
                                break;
                            case 2:
                                _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
                                _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 100));
                                _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                                _label.Text = "Sõida";
                                delay = 4000;
                                break;
                        }
                    }
                    else if (_currentMode == Mode.Night)
                    {
                        _label.Text = "Öörežiim";

                        switch (index)
                        {
                            case 0:
                                _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
                                _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 0));
                                _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));
                                break;
                            case 1:
                                _ellipse1.Fill = new SolidColorBrush(Color.FromRgb(150, 100, 100));
                                _ellipse2.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                                _ellipse3.Fill = new SolidColorBrush(Color.FromRgb(100, 150, 100));
                                break;
                        }
                    }
                });

                index = (index + 1) % ((_currentMode == Mode.Night) ? 2 : 3);
                await Task.Delay(delay, token);
            }
        }, token);
    }

    private void StopMode()
    {
        _currentMode = Mode.Manual;
        if (_modeCts != null)
        {
            _modeCts.Cancel();
            _modeCts.Dispose();
            _modeCts = null;
        }
    }

    private void UpdateModeButtons()
    {
        foreach (Button but in _vslMode.Children)
        {
            if ((Mode)but.ZIndex == _currentMode)
            {
                but.BackgroundColor = Colors.LightGray;
                but.TextColor = Colors.Black;
            }
            else
            {
                but.BackgroundColor = Colors.Black;
                but.TextColor = Colors.White;
            }
        }
    }
}