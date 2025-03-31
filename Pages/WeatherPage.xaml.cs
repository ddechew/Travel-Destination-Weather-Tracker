namespace DestinationsApp;

using DestinationsApp.Services;
using DestinationsApp.Services.Models;

using System.Globalization;

public partial class WeatherPage : ContentPage
{
    private WeatherResult _weatherResult;
    private string _cityName;
    private ForecastDay? _selectedDay = null;

    public WeatherPage(string cityName)
    {
        InitializeComponent();
        _cityName = cityName;
        LoadWeather();
    }

    private async void LoadWeather()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            MainContent.IsVisible = false;

            var service = new WeatherService();
            _weatherResult = await service.GetWeatherAsync(_cityName);

            if (_weatherResult == null)
            {
                await DisplayAlert("Error", "Could not load weather data.", "OK");
                await Navigation.PopAsync(); 
                return;
            }

            RenderHourlyForecast(_weatherResult.Forecast.ForecastDays.FirstOrDefault());
            CityLabel.Text = $"{_weatherResult.Location.Name}, {_weatherResult.Location.Country}";
            TemperatureLabel.Text = $"{_weatherResult.Current.TempC}°C";
            DescriptionLabel.Text = $"Condition: {_weatherResult.Current.Condition.Text}";
            ConditionIcon.Source = "https:" + _weatherResult.Current.Condition.Icon;
            ChanceOfRainLabel.Text = $"Chance of Rain: {_weatherResult.Forecast.ForecastDays.First().Day.ChanceOfRain}%";
            HumidityLabel.Text = $"Humidity: {_weatherResult.Current.Humidity}%";
            WindSpeedLabel.Text = $"Wind: {_weatherResult.Current.WindKph} kph";

            for (int i = 0; i < _weatherResult.Forecast.ForecastDays.Count; i++)
            {
                var day = _weatherResult.Forecast.ForecastDays[i];
                if (DateTime.TryParse(day.Date, out var parsedDate))
                {
                    day.DisplayDate = i == 0 && parsedDate.Date == DateTime.Now.Date
                        ? "Today"
                        : parsedDate.ToString("dddd", CultureInfo.InvariantCulture); 
                }
            }

            ForecastListView.ItemsSource = _weatherResult.Forecast.ForecastDays;

            var todayForecast = _weatherResult.Forecast.ForecastDays.FirstOrDefault();
            if (todayForecast != null)
            {
                MaxTempLabel.Text = $"Max: {todayForecast.Day.MaxTempC}°C";
                MinTempLabel.Text = $"Min: {todayForecast.Day.MinTempC}°C";
                var now = DateTime.Now;
                var currentHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

                var filteredHours = todayForecast.Hour
                    .Where(h =>
                    {
                        if (DateTime.TryParse(h.Time, out var hourTime))
                            return hourTime >= currentHour;
                        return false;
                    })
                    .ToList();

                foreach (var hour in filteredHours)
                {
                    if (!hour.Condition.Icon.StartsWith("https"))
                        hour.Condition.Icon = "https:" + hour.Condition.Icon;
                }

                HourlyForecastLayout.Children.Clear();

                foreach (var hour in filteredHours)
                {
                    var layout = new VerticalStackLayout
                    {
                        Spacing = 8,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    layout.Children.Add(new Label
                    {
                        Text = DateTime.Parse(hour.Time).ToString("HH:mm"),
                        FontSize = 18,
                        TextColor = Colors.White,
                        HorizontalTextAlignment = TextAlignment.Center
                    });

                    layout.Children.Add(new Image
                    {
                        Source = hour.Condition.Icon,
                        WidthRequest = 50,
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.Center
                    });

                    layout.Children.Add(new Label
                    {
                        Text = $"{hour.TempC}°C",
                        FontSize = 16,
                        TextColor = Colors.DodgerBlue,
                        HorizontalTextAlignment = TextAlignment.Center
                    });

                    layout.Children.Add(new Label
                    {
                        Text = $"💧 {hour.ChanceOfRain}%",
                        FontSize = 12,
                        TextColor = Colors.LightSkyBlue,
                        HorizontalTextAlignment = TextAlignment.Center
                    });


                    HourlyForecastLayout.Children.Add(layout);

                    LoadingIndicator.IsVisible = false;
                    MainContent.IsVisible = true;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnForecastSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ForecastDay selectedDay)
        {
            _selectedDay = selectedDay;

            var parsedDate = DateTime.Parse(selectedDay.Date);
            Title = parsedDate.Date == DateTime.Now.Date ? "Today" : parsedDate.ToString("dddd", CultureInfo.InvariantCulture);

            var mainTemp = selectedDay.Hour?.MaxBy(h => h.TempC); 
            TemperatureLabel.Text = mainTemp != null ? $"{mainTemp.TempC}°C" : $"{selectedDay.Day.AvgTempC}°C";

            DescriptionLabel.Text = $"Condition: {selectedDay.Day.Condition.Text}";
            ConditionIcon.Source = selectedDay.Day.Condition.FullIcon;
            ChanceOfRainLabel.Text = $"Chance of Rain: {selectedDay.Day.ChanceOfRain}%";
            HumidityLabel.IsVisible = parsedDate.Date == DateTime.Now.Date;
            HumidityLabel.Text = $"Humidity: {_weatherResult.Current.Humidity}%";
            WindSpeedLabel.IsVisible = parsedDate.Date == DateTime.Now.Date;
            WindSpeedLabel.Text = $"Wind: {_weatherResult.Current.WindKph} kph"; 

            MaxTempLabel.Text = $"Max: {selectedDay.Day.MaxTempC}°C";
            MinTempLabel.Text = $"Min: {selectedDay.Day.MinTempC}°C";

            RenderHourlyForecast(selectedDay);

            await MainScrollView.ScrollToAsync(0, 0, true);
        }

        ((CollectionView)sender).SelectedItem = null;
    }

    private void RenderHourlyForecast(ForecastDay forecastDay)
    {
        var now = DateTime.Now;
        var currentHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

        var filteredHours = forecastDay.Hour
            .Where(h =>
            {
                if (DateTime.TryParse(h.Time, out var hourTime))
                {
                    if (_selectedDay != null && DateTime.TryParse(_selectedDay.Date, out var selectedDate))
                    {
                        bool isToday = selectedDate.Date == DateTime.Now.Date;
                        return !isToday || hourTime >= currentHour;
                    }
                    return false;
                }
                return false;
            })
            .ToList();

        foreach (var hour in filteredHours)
        {
            if (!hour.Condition.Icon.StartsWith("https"))
                hour.Condition.Icon = "https:" + hour.Condition.Icon;
        }

        HourlyForecastLayout.Children.Clear();

        foreach (var hour in filteredHours)
        {
            var layout = new VerticalStackLayout
            {
                Spacing = 8,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            layout.Children.Add(new Label
            {
                Text = DateTime.Parse(hour.Time).ToString("HH:mm"),
                FontSize = 18,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            });

            layout.Children.Add(new Image
            {
                Source = hour.Condition.Icon,
                WidthRequest = 50,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Children.Add(new Label
            {
                Text = $"{hour.TempC}°C",
                FontSize = 16,
                TextColor = Colors.DodgerBlue,
                HorizontalTextAlignment = TextAlignment.Center
            });

            layout.Children.Add(new Label
            {
                Text = $"💧 {hour.ChanceOfRain}%",
                FontSize = 12,
                TextColor = Colors.LightSkyBlue,
                HorizontalTextAlignment = TextAlignment.Center
            });

            HourlyForecastLayout.Children.Add(layout);
        }
    }

}
