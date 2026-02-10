namespace Example;

public partial class TextPage : ContentPage
{
	private Editor _editor;
	List<string> _buttons = ["Tagasi", "Avaleht", "Edasi"];

	public TextPage()
	{
		var lbl = new Label
		{
			Text = "Pealkiri",
			FontSize = 36,
			TextColor = Colors.Black,
			HorizontalOptions = LayoutOptions.Center,
			FontAttributes = FontAttributes.Bold,
		};

        _editor = new Editor
		{
			Placeholder = "Sisesta tekst...",
			PlaceholderColor = Colors.Red,
			FontSize = 18,
			FontAttributes = FontAttributes.Italic,
		};
        _editor.TextChanged += (sender, e) =>
		{
			lbl.Text = _editor.Text;
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
					Navigation.PopAsync();
				}
				else if (but.ZIndex == 1)
				{
					Navigation.PopToRootAsync();
				}
				else
				{
                    Navigation.PushAsync(new FigurePage());
                }
			};


            hsl.Add(but);
		}

		var vsl = new VerticalStackLayout
		{
			Spacing = 15,
			Padding = 20,
			Children = { lbl, _editor, hsl },
			HorizontalOptions = LayoutOptions.Center
		};

		Content = vsl;
	}

	public async void Btn_Clicked(object? sender, EventArgs e)
	{
		IEnumerable<Locale> locales = await TextToSpeech.GetLocalesAsync();

		SpeechOptions options = new()
		{
			Pitch = 1,
			Volume = 0.5f,
			Locale = locales.FirstOrDefault(l => l.Language == "et-EE"),
        };

		var text = _editor.Text;
		if (string.IsNullOrEmpty(text))
		{
			await DisplayAlertAsync("Viga", "Palun sisesta tekst", "OK");
			return;
		}

		try
		{
			await TextToSpeech.SpeakAsync(text, options);
		}
		catch (Exception ex)
		{
            await DisplayAlertAsync("TTS Viga", ex.Message, "OK");
        }
    }
}