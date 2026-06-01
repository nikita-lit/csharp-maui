using Example.CityExplorer.ViewModels;

namespace Example.CityExplorer.Views;

public partial class SettingsPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
