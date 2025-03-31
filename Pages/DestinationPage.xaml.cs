namespace DestinationsApp;

using DestinationsApp.Models;

using System.Collections.ObjectModel;

public partial class DestinationPage : ContentPage
{
    private bool isEditing;
    private string pageTitle;
    private string selectedStatus;
    private string selectedCountry;
    private string selectedCity;
    private string cityToPreselect = null;
    private string originalCountry;
    private string originalCity;
    private bool isLoading;
    private Destination editingDestination;

    public ObservableCollection<Destination> Destinations { get; set; }

    public ObservableCollection<string> Countries { get; set; }

    public ObservableCollection<string> StatusOptions { get; set; }

    public bool IsEditing
    {
        get => isEditing;
        set
        {
            isEditing = value;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(IsNotEditing));
        }
    }

    public bool IsLoading
    {
        get => isLoading;
        set
        {
            if (isLoading != value)
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }
    }

    public bool IsNotLoading => !isLoading;

    public string SelectedCountry
    {
        get => selectedCountry;
        set
        {
            if (selectedCountry != value)
            {
                selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
                _ = LoadCitiesForSelectedCountry(selectedCountry);
            }
        }
    }

    public string SelectedCity
    {
        get => selectedCity;
        set
        {
            selectedCity = value;
            OnPropertyChanged(nameof(SelectedCity));
        }
    }


    public string PageTitle
    {
        get => pageTitle;
        set
        {
            pageTitle = value;
            OnPropertyChanged(nameof(PageTitle));
        }
    }

    public string SelectedStatus
    {
        get => selectedStatus;
        set
        {
            selectedStatus = value;
            OnPropertyChanged(nameof(SelectedStatus));
        }
    }

    public bool IsNotEditing => !isEditing;

    private readonly CountryService countryService = new();

    public ObservableCollection<string> Cities { get; set; } = new();

    public DestinationPage(ObservableCollection<Destination> destinations, List<string> countryNames, Destination destinationToEdit = null)
    {
        InitializeComponent();
        Destinations = destinations;
        Countries = new ObservableCollection<string>(countryNames);

        StatusOptions = new ObservableCollection<string>
        {
            "Planned", "Ongoing", "Completed", "Cancelled"
        };

        BindingContext = this;

        if (destinationToEdit != null)
        {
            IsEditing = true;
            editingDestination = destinationToEdit;
            PageTitle = "Edit Destination";

            originalCountry = destinationToEdit.Country;
            originalCity = destinationToEdit.City;

            SelectedCountry = destinationToEdit.Country;
            cityToPreselect = destinationToEdit.City;

            StartDatePicker.Date = editingDestination.StartDate;
            DurationEntry.Text = editingDestination.Duration.ToString();
            PurposeEntry.Text = editingDestination.Purpose;
            RatingEntry.Text = editingDestination.Rating.ToString();
            StatusPicker.SelectedItem = editingDestination.Status;

            SelectedStatus = editingDestination.Status;
        }
        else
        {
            IsEditing = false;
            PageTitle = "Add New Destination";
        }
    }

    private async void OnAddDestinationClicked(object sender, EventArgs e)
    {
        if (SelectedCountry == null || SelectedCity == null ||
            string.IsNullOrWhiteSpace(DurationEntry.Text) || StatusPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please fill in all fields!", "OK");
            return;
        }

        string country = SelectedCountry;
        string city = SelectedCity;
        DateTime startDate = StartDatePicker.Date;
        int duration = int.TryParse(DurationEntry.Text, out int d) ? d : 0;
        string purpose = string.IsNullOrWhiteSpace(PurposeEntry.Text) ? "No purpose specified" : PurposeEntry.Text;
        double rating = double.TryParse(RatingEntry.Text, out double r) ? r : 0;
        string status = StatusPicker.SelectedItem.ToString();

        if (isEditing && editingDestination != null)
        {
            editingDestination.Country = country;
            editingDestination.City = city;
            editingDestination.StartDate = startDate;
            editingDestination.Duration = duration;
            editingDestination.Purpose = purpose;
            editingDestination.Rating = rating;
            editingDestination.Status = status;

                    bool locationChanged =
            !string.Equals(editingDestination.Country, originalCountry, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(editingDestination.City, originalCity, StringComparison.OrdinalIgnoreCase);

            if (locationChanged)
            {
                bool confirm = await DisplayAlert(
                    "Change Location",
                    "Changing the country or city will delete all related expenses. Proceed?",
                    "Yes", "No");

                if (!confirm)
                    return;

                await App.Database.DeleteExpensesByDestinationIdAsync(editingDestination.Id);
            }

            await App.Database.UpdateDestinationAsync(editingDestination);
        }
        else
        {
            var newDestination = new Destination(country, city, startDate, duration, purpose, rating, status);
            await App.Database.AddDestinationAsync(newDestination);
            Destinations.Add(newDestination);
        }

        await Navigation.PopAsync();
    }
    private async Task LoadCitiesForSelectedCountry(string selectedCountry, string cityToSelect = null)
    {
        Cities.Clear();

        if (!string.IsNullOrWhiteSpace(selectedCountry))
        {
            var cities = await countryService.GetCitiesByCountryAsync(selectedCountry);
            foreach (var city in cities)
                Cities.Add(city);

            if (!string.IsNullOrWhiteSpace(cityToSelect) && Cities.Contains(cityToSelect))
            {
                SelectedCity = cityToSelect;
            }
        }

    }

    private async void OnExpensesClicked(object sender, EventArgs e)
    {
        if (IsEditing && editingDestination != null)
        {
            await Navigation.PushAsync(new ExpensesPage(editingDestination));
        }
        else
        {
            await DisplayAlert("Missing Data", "Please save the destination first.", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (IsEditing && !string.IsNullOrWhiteSpace(SelectedCountry))
        {
            IsLoading = true;

            Task.Run(async () =>
            {
                var cities = await countryService.GetCitiesByCountryAsync(SelectedCountry);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Cities.Clear();
                    foreach (var city in cities)
                        Cities.Add(city);

                    if (!string.IsNullOrWhiteSpace(cityToPreselect) && Cities.Contains(cityToPreselect))
                        SelectedCity = cityToPreselect;
                });

                IsLoading = false;
            });
        }
    }

    private async void OnCheckWeatherClicked(object sender, EventArgs e)
    {
        if (CityPicker.SelectedItem is string selectedCity)
        {
            await Navigation.PushAsync(new WeatherPage(selectedCity)); 
        }
        else
        {
            await DisplayAlert("Warning", "Please select a city first.", "OK");
        }
    }
}
