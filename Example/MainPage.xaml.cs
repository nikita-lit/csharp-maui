namespace Example
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;
            CounterLabel.Text = "Nupp oli vajutatud";
            DotNetBot.Rotation += 20;
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                if (count >= 5)
                {
                    CounterBtn.BackgroundColor = Colors.Red;
                    CounterBtn.TextColor = Colors.White;
                }
            CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
            var rnd = new Random();
            var color = Color.FromRgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            BackgroundColor = color;// muudab kogu lehe taustavärvi
            ResetBtn.BackgroundColor = color;// muudab reset nupu taustavärvi
        }

        private void ResetBtn_Clicked(object sender, EventArgs e)
        {
            count = 0;
            CounterBtn.Text = "Vajuta siia";
            CounterLabel.Text = "Alustame uuesti!";
            DotNetBot.Rotation = 0;
            DotNetBot.IsVisible = true;
            ResetBtn.ClearValue(BackgroundColorProperty);// eemaldab reset nupu taustavärvi
            CounterBtn.ClearValue(BackgroundColorProperty);// eemaldab counter nupu taustavärvi
            CounterBtn.ClearValue(Button.TextColorProperty);// eemaldab counter nupu tekstivärvi
            BackgroundColor = Colors.White;// muudab kogu lehe taustavärvi valgeks
            if (DotNetBot.HorizontalOptions == LayoutOptions.Start)
            {
                DotNetBot.HorizontalOptions = LayoutOptions.End;
            }
            else
            {
                DotNetBot.HorizontalOptions = LayoutOptions.Start;
            }

        }

        private async void Ülesse_Clicked(object sender, EventArgs e)
        {
            // Hüppame üles!
            // 0 = X (vasak-parem ei muutu)
            // -75 = Y (liigume 75 ühikut ülespoole)
            // 250 = Aeg millisekundites (kui kiiresti hüppab)
            await Bot.TranslateTo(0, -75, 250);
        }

        private async void Alla_Clicked(object sender, EventArgs e)
        {
            // Tuleme tagasi algusesse (0, 0)
            await Bot.TranslateTo(0, 0, 250);
        }
    }
}
