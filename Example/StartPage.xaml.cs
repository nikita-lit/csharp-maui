using System;
using Example.TicTacToe;

namespace Example;

public partial class StartPage : ContentPage
{
	VerticalStackLayout layout;
	ScrollView scrollView;
	public List<string> pageNames = new List<string>() { 
		"Tekst", 
		"Kujund", 
		"Valgusfoor", 
		"RGB", 
		"Lumememm", 
		"PopUp", 
		"Matemaatika Test", 
		"Trips Traps Trull", 
		"Sõbrade kontaktandmed",
		"Dünaamiline ListView",
		"Euroopa riigid",
	};
	
	private List<Type> pageTypes = new List<Type>() { 
		typeof(TextPage), 
		typeof(FigurePage), 
		typeof(ValgusfoorPage), 
		typeof(RGBPage), 
		typeof(LumememmPage), 
		typeof(PopUpPage), 
		typeof(MathTest), 
		typeof(TicTacToeStart),
		typeof(Contacts),
		typeof(ListViewPage),
		typeof(EuropeCountriesPage),
	};

	public StartPage()
	{
		Title = "Avaleht";
		layout = new VerticalStackLayout()
		{
			Spacing = 25,
			Padding = new Thickness(30, 0),
			VerticalOptions = LayoutOptions.Center
		};

		for (int i = 0; i < pageTypes.Count; i++)
		{
			var button = new Button() { Text = pageNames[i], FontFamily = "NunitoSansRegular", FontSize = 18, FontAttributes = FontAttributes.Bold };
			int index = i;
			button.Clicked += async (sender, args) =>
			{
				var page = (ContentPage)Activator.CreateInstance(pageTypes[index]);
				await Navigation.PushAsync(page);
			};
			layout.Children.Add(button);
		}
		
		scrollView = new ScrollView() { Content = layout };
		Content = scrollView;
    }
}