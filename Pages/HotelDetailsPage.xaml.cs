namespace DestinationsApp;

using DestinationsApp.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class HotelDetailsPage : ContentPage, INotifyPropertyChanged
{
    private readonly Hotel _hotel;
    private readonly Destination _destination;
    private int _maxDuration;
    private int _selectedDuration;
    private int _usedDays;
    private int _destinationDuration;
    private Expense _existingExpense;
    private bool _isEditMode;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Color RemainingLabelColor => MaxDuration > 0 ? Colors.LimeGreen : Colors.Orange;

    public int SelectedDuration
    {
        get => _selectedDuration;
        set
        {
            if (_selectedDuration != value)
            {
                _selectedDuration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedDurationLabel));
            }
        }
    }

    public bool CanProceed => SelectedDuration + _usedDays <= _destinationDuration;

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            _isEditMode = value;
            OnPropertyChanged();
        }
    }

    public int MaxDuration
    {
        get => _maxDuration;
        set
        {
            if (_maxDuration != value)
            {
                _maxDuration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RemainingLabel));
                OnPropertyChanged(nameof(UsageLabel));
                OnPropertyChanged(nameof(RemainingLabelColor));
            }
        }
    }

    public string SelectedDurationLabel => $"Selected: {SelectedDuration} night{(SelectedDuration == 1 ? "" : "s")}";

    public string RemainingLabel
    {
        get
        {
            if (IsEditMode)
            {
                return $"Editing your stay: {SelectedDuration} night{(SelectedDuration == 1 ? "" : "s")} selected.";
            }

            return MaxDuration > 0
                ? $"You can book up to {MaxDuration} more night{(MaxDuration == 1 ? "" : "s")}."
                : "No nights left in your plan. You’re booking extra (emergency) nights.";
        }
    }
    public string UsageLabel => $"Used {_usedDays} out of {_destinationDuration} days";
    public string HotelName { get; set; }
    public string Address { get; set; }
    public string Price { get; set; }

    public HotelDetailsPage(Hotel hotel, Destination destination)
    {
        InitializeComponent();
        _hotel = hotel;
        _destination = destination;

        HotelName = _hotel.Name;
        Address = $"{_hotel.CityCode}, {_hotel.CountryCode}";
        Price = $"{_hotel.Price} BGN";

        BindingContext = this;

        MaxDuration = 1;
        SelectedDuration = 1;

        RefreshAvailability();
    }
    private async void RefreshAvailability()
    {
        var expenses = await App.Database.GetExpensesByDestinationIdAsync(_destination.Id);
        _existingExpense = expenses.FirstOrDefault(e => e.Type == "Hotel" && e.HotelName == _hotel.Name);
        _destinationDuration = _destination.Duration;

        _usedDays = expenses
            .Where(e => e.Type == "Hotel" && e.HotelName != _hotel.Name)
            .Sum(e => e.Duration);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            MaxDuration = Math.Max(0, _destinationDuration - _usedDays);
            SelectedDuration = _existingExpense?.Duration ?? (MaxDuration > 0 ? MaxDuration : 1);
            IsEditMode = _existingExpense != null;

            OnPropertyChanged(nameof(RemainingLabel));
            OnPropertyChanged(nameof(UsageLabel));
            OnPropertyChanged(nameof(RemainingLabelColor));
        });
    }
    private async void OnAddExpenseClicked(object sender, EventArgs e)
    {
        if (SelectedDuration <= 0)
        {
            await DisplayAlert("Invalid Duration", "Please select at least 1 day.", "OK");
            return;
        }

        var existingExpenses = await App.Database.GetExpensesByDestinationIdAsync(_destination.Id);
        var existingHotelExpense = existingExpenses
            .FirstOrDefault(e => e.Type == "Hotel" && e.HotelName == _hotel.Name);

        int currentHotelDuration = existingHotelExpense?.Duration ?? 0;
        int otherHotelsDuration = existingExpenses
            .Where(e => e.Type == "Hotel" && e.HotelName != _hotel.Name)
            .Sum(e => e.Duration);

        int projectedTotal = otherHotelsDuration + currentHotelDuration + SelectedDuration;

        if (projectedTotal > _destinationDuration)
        {
            int excess = projectedTotal - _destinationDuration;

            bool proceed = await DisplayAlert(
                "Duration Warning",
                $"You're exceeding your trip by {excess} day{(excess > 1 ? "s" : "")}.\nDo you want to continue anyway?",
                "Yes, Proceed", "Cancel");

            if (!proceed)
                return;
        }

        if (existingHotelExpense != null)
        {
            existingHotelExpense.Duration += SelectedDuration;
            await App.Database.SaveExpenseAsync(existingHotelExpense);

            await DisplayAlert("Updated", $"Hotel '{_hotel.Name}' updated. Total nights: {existingHotelExpense.Duration}", "OK");
        }
        else
        {
            var expense = new Expense
            {
                Type = "Hotel",
                HotelName = _hotel.Name,
                BookedPrice = _hotel.Price,
                Duration = SelectedDuration,
                City = _hotel.CityCode,
                Country = _hotel.CountryCode,
                DestinationId = _destination.Id,
                Latitude = _hotel.Latitude,
                Longitude = _hotel.Longitude
            };

            await App.Database.SaveExpenseAsync(expense);

            await DisplayAlert("Added", $"Hotel '{_hotel.Name}' added for {SelectedDuration} nights!", "OK");
        }

        await Navigation.PopAsync();
    }
    private async void OnEditExpenseClicked(object sender, EventArgs e)
    {
        if (_existingExpense == null)
            return;

        ((Button)sender).IsEnabled = false; // Prevent spam clicks

        try
        {
            if (SelectedDuration <= 0)
            {
                bool confirmDelete = await DisplayAlert(
                    "Remove Stay?",
                    "You’ve set the duration to 0. Do you want to delete this hotel from your expenses?",
                    "Yes, Delete",
                    "Cancel"
                );

                if (confirmDelete)
                {
                    await App.Database.DeleteExpenseAsync(_existingExpense);
                    await DisplayAlert("Deleted", "Hotel expense has been removed.", "OK");

                    await Navigation.PopAsync();
                }

                return; 
            }

            int projectedTotal = _usedDays + SelectedDuration;

            if (projectedTotal > _destinationDuration)
            {
                int excessDays = projectedTotal - _destinationDuration;

                bool proceed = await DisplayAlert(
                    "Duration Warning",
                    $"You're exceeding your trip by {excessDays} day{(excessDays > 1 ? "s" : "")}.\nDo you want to update anyway?",
                    "Yes, Update",
                    "Cancel");

                if (!proceed) return;
            }

            _existingExpense.Duration = SelectedDuration;
            await App.Database.SaveExpenseAsync(_existingExpense);

            await DisplayAlert("Updated", $"Duration updated to {SelectedDuration} night{(SelectedDuration == 1 ? "" : "s")}.", "OK");
            await Navigation.PopAsync();
        }
        finally
        {
            ((Button)sender).IsEnabled = true; 
        }
    }
    private async void OnViewInMapsClicked(object sender, EventArgs e)
    {
        try
        {
            var url = $"https://www.google.com/maps/search/?api=1&query={_hotel.Latitude},{_hotel.Longitude}";
            await Launcher.Default.OpenAsync(url);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to open Google Maps: {ex.Message}", "OK");
        }
    }
}
