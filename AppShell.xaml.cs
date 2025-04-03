using DestinationsApp.Models;
using DestinationsApp.Services;

using System.Collections.ObjectModel;

namespace DestinationsApp;

public partial class AppShell : Shell
{
    private Destination _currentDestination;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("MainPage", typeof(MainPage)); 
        Routing.RegisterRoute(nameof(ExpensesPage), typeof(ExpensesPage));
        Routing.RegisterRoute(nameof(WeatherPage), typeof(WeatherPage));
        Routing.RegisterRoute(nameof(DestinationPage), typeof(DestinationPage));
    }

    public void SetCurrentDestination(Destination destination)
    {
        _currentDestination = destination;
    }

    private async void OnGoToExpenses(object sender, EventArgs e)
    {
        if (_currentDestination != null)
            await Shell.Current.GoToAsync(nameof(ExpensesPage), true, new Dictionary<string, object>
            {
                { "destination", _currentDestination }
            });
        else
            await Shell.Current.DisplayAlert("Error", "No destination selected.", "OK");
    }

    private async void OnGoToWeather(object sender, EventArgs e)
    {
        if (_currentDestination != null)
            await Shell.Current.GoToAsync(nameof(WeatherPage), true, new Dictionary<string, object>
            {
                { "cityName", _currentDestination.City }
            });
        else
            await Shell.Current.DisplayAlert("Error", "No destination selected.", "OK");
    }

    private async void OnGoToAddDestination(object sender, EventArgs e)
    {
        var countries = await new CountryService().GetCountryNamesAsync();
        var destinations = await App.Database.GetDestinationsAsync();

        await Shell.Current.Navigation.PopToRootAsync();
        await Shell.Current.Navigation.PushAsync(new DestinationPage(
            new ObservableCollection<Destination>(destinations),
            countries.ToList()
        ));
    }

    private async void OnGoToDestinations(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopToRootAsync(); 
        await Shell.Current.Navigation.PushAsync(new MainPage()); 
    }
}
