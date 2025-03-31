using DestinationsApp.Models;
using DestinationsApp.Services; 

using System.Collections.ObjectModel;

namespace DestinationsApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Destination> Destinations { get; set; } = new();
        private readonly DatabaseService dbService;

        public static List<string> StatusList { get; } = new List<string>
        {
            "Planned",
            "Ongoing",
            "Completed",
            "Cancelled"
        };

        private readonly CountryService countryService = new(); 

        public List<string> CountryNames { get; private set; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "destinations.db3");
            dbService = new DatabaseService(dbPath);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            CountryNames = await countryService.GetCountryNamesAsync();

            var destinationsFromDb = await dbService.GetDestinationsAsync();
            Destinations.Clear();

            foreach (var d in destinationsFromDb)
                Destinations.Add(d);
        }

        public async void DeleteBtn(Destination currentDestination)
        {
            if (currentDestination != null && Destinations.Contains(currentDestination))
            {
                Destinations.Remove(currentDestination);
                await dbService.DeleteDestinationAsync(currentDestination);
            }
        }
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Destination destination)
            {
                bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this destination?", "Yes", "No");
                if (!confirm) return;

                await App.Database.DeleteDestinationAsync(destination);

                Destinations.Remove(destination);
            }
        }

        public async void AddNewDestination(object sender, EventArgs e)
        {
            if (CountryNames.Count == 0)
            { 
                CountryNames = await countryService.GetCountryNamesAsync();
            }

            await Navigation.PushAsync(new DestinationPage(Destinations, CountryNames));
        }
        private async void EditDestination(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Destination destination)
            {
                await Navigation.PushAsync(new DestinationPage(Destinations, CountryNames, destination));
            }
        }
        private async void OnActiveClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Destination destination)
            {
                destination.SetActive();
                await App.Database.UpdateDestinationAsync(destination);
            }
        }
        private async void OnCompletedClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Destination destination)
            {
                destination.SetCompleted();
                await App.Database.UpdateDestinationAsync(destination);
            }
        }
    }
}
