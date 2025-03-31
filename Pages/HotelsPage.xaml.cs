namespace DestinationsApp;

using DestinationsApp.Models;
using DestinationsApp.Services;

using System.ComponentModel;

public partial class HotelsPage : ContentPage, INotifyPropertyChanged
{
    private readonly AmadeusService _amadeusService = new();
    private readonly Destination _destination;
    private List<Hotel> _hotels;

    public HotelsPage(Destination destination)
    {
        InitializeComponent();
        _destination = destination;
        BindingContext = this;

        LoadHotels(destination.City);
    }
    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsLoading { get; set; } = false;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void LoadHotels(string city)
    {
        try
        {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));

            string cityCode = await _amadeusService.GetCityCodeAsync(city);
            _hotels = await _amadeusService.GetHotelsByCityCodeAsync(cityCode);
            HotelsListView.ItemsSource = _hotels;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_hotels == null)
            return;

        string query = e.NewTextValue?.ToLowerInvariant().Trim() ?? "";

        if (string.IsNullOrWhiteSpace(query))
        {
            ApplySorting(_hotels);
            return;
        }

        var filtered = _hotels.Where(h =>
            (!string.IsNullOrEmpty(h.Name) && h.Name.ToLowerInvariant().Contains(query)) ||
            (!string.IsNullOrEmpty(h.CityCode) && h.CityCode.ToLowerInvariant().Contains(query)) ||
            (!string.IsNullOrEmpty(h.CountryCode) && h.CountryCode.ToLowerInvariant().Contains(query))
        ).ToList();

        ApplySorting(filtered);
    }

    private void ApplySorting(List<Hotel> hotelList)
    {
        if (SortPicker.SelectedIndex == 0)
            HotelsListView.ItemsSource = hotelList.OrderBy(h => h.Price).ToList();
        else if (SortPicker.SelectedIndex == 1)
            HotelsListView.ItemsSource = hotelList.OrderByDescending(h => h.Price).ToList();
        else
            HotelsListView.ItemsSource = hotelList; 
    }
    private async void OnHotelDetailsClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Hotel selectedHotel)
        {
            await Navigation.PushAsync(new HotelDetailsPage(selectedHotel, _destination));
        }
    }
    private void OnSortChanged(object sender, EventArgs e)
    {
        if (_hotels == null) return;

        if (SortPicker.SelectedIndex == 0) 
            HotelsListView.ItemsSource = _hotels.OrderBy(h => h.Price).ToList();
        else if (SortPicker.SelectedIndex == 1) 
            HotelsListView.ItemsSource = _hotels.OrderByDescending(h => h.Price).ToList();
    }
}