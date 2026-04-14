using System.Collections.ObjectModel;

namespace Example;

public partial class EuropeCountriesPage : ContentPage
{
    private ObservableCollection<Country> _countries;
    private string _selectedImagePath = "";
    private Country _lastCountry;

    public EuropeCountriesPage()
    {
        InitializeComponent();

        _countries = new ObservableCollection<Country>
        {
            new Country { Name = "Eesti", Capital = "Tallinn", Population = 1331824, Flag = "estonia.png" },
            new Country { Name = "Soome", Capital = "Helsingi", Population = 5540720, Flag = "finland.png" },
            new Country { Name = "Läti", Capital = "Riia", Population = 1884000, Flag = "latvia.png" },
            new Country { Name = "Saksamaa", Capital = "Berliin", Population = 83200000, Flag = "germany.png" }
        };

        CountriesListView.ItemsSource = _countries;
    }

    private async void PickImage_Clicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null)
            {
                _selectedImagePath = photo.FullPath;
                ImageStatusLabel.Text = "";
                FlagPreview.Source = _selectedImagePath;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Viga", "Pildi valimine ebaõnnestus: " + ex.Message, "OK");
        }
    }

    private async void List_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Country selectedCountry)
        {
            if (_lastCountry == selectedCountry)
            {
                CountriesListView.SelectedItem = null;
                _lastCountry = null;
                ClearEntries();
            }
            else
            {
                _lastCountry = selectedCountry;
                await DisplayAlertAsync("Riigi info", 
                    $"Riik: {selectedCountry.Name}\nPealinn: {selectedCountry.Capital}\nRahvaarv: {selectedCountry.Population} inimest", 
                    "Sulge");
            }
        }
    }

    private void CountriesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Country selectedCountry)
        {
            NameEntry.Text = selectedCountry.Name;
            CapitalEntry.Text = selectedCountry.Capital;
            PopulationEntry.Text = selectedCountry.Population.ToString();

            _selectedImagePath = selectedCountry.Flag;
            ImageStatusLabel.Text = "";
            FlagPreview.Source = _selectedImagePath;
        }
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        string newName = NameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(newName))
        {
            await DisplayAlertAsync("Viga", "Palun sisesta riigi nimi!", "OK");
            return;
        }

        bool countryExists = _countries.Any(c => c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase));

        if (countryExists)
            await DisplayAlertAsync("Viga", "See riik on juba nimekirjas!", "OK");
        else
        {
            int population = 0;
            int.TryParse(PopulationEntry.Text, out population);

            string flagSource = !string.IsNullOrWhiteSpace(_selectedImagePath) ? _selectedImagePath : "bob.png";

            _countries.Add(new Country 
            { 
                Name = newName, 
                Capital = CapitalEntry.Text, 
                Population = population, 
                Flag = flagSource
            });

            ClearEntries();
        }
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        if (CountriesListView.SelectedItem is Country selectedCountry)
        {
            selectedCountry.Name = NameEntry.Text;
            selectedCountry.Capital = CapitalEntry.Text;
            
            int.TryParse(PopulationEntry.Text, out int population);
            selectedCountry.Population = population;
            
            selectedCountry.Flag = _selectedImagePath;

            CountriesListView.SelectedItem = null;
            ClearEntries();
            await DisplayAlertAsync("Info", "Muudatused salvestatud!", "OK");
        }
        else
        {
            await DisplayAlertAsync("Viga", "Palun vali nimekirjast riik, mida soovite muuta!", "OK");
        }
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        if (CountriesListView.SelectedItem is Country selectedCountry)
        {
            bool confirm = await DisplayAlertAsync("Kinnitus", $"Kas olete kindel, et soovite riigi {selectedCountry.Name} kustutada?", "Jah", "Ei");
            if (confirm)
            {
                _countries.Remove(selectedCountry);
                ClearEntries();
                CountriesListView.SelectedItem = null;
            }
        }
        else
        {
            await DisplayAlertAsync("Viga", "Palun vali nimekirjast riik, mida soovite kustutada!", "OK");
        }
    }

    private void ClearEntries()
    {
        NameEntry.Text = string.Empty;
        CapitalEntry.Text = string.Empty;
        PopulationEntry.Text = string.Empty;
        _selectedImagePath = "";
        ImageStatusLabel.Text = "Pilti pole valitud (kasutatakse vaikimisi pilti)";
        ImageStatusLabel.TextColor = Colors.Gray;
        FlagPreview.Source = "bob.png";
        _lastCountry = null;
    }
}