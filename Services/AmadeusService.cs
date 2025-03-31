using DestinationsApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DestinationsApp.Services;

public class AmadeusService
{
    private readonly HttpClient _httpClient;
    private string _accessToken;
    private readonly string ClientId;
    private readonly string ClientSecret;
    private static readonly string BaseUrl = "https://test.api.amadeus.com/v1";
    private static readonly Random random = new Random();

    public AmadeusService()
    {
        _httpClient = new HttpClient();

        ClientId = ConfigurationService.GetAmadeusValue("ClientId");
        ClientSecret = ConfigurationService.GetAmadeusValue("ClientSecret");
    }

    private async Task AuthenticateAsync()
    {
        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", ClientId },
            { "client_secret", ClientSecret }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/security/oauth2/token")
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("Authentication failed: " + content);

        var json = JsonDocument.Parse(content);
        _accessToken = json.RootElement.GetProperty("access_token").GetString();
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            await AuthenticateAsync();
        }
    }

    public async Task<string> GetCityCodeAsync(string cityName)
    {
        await EnsureAuthenticatedAsync();

        var encodedCity = Uri.EscapeDataString(cityName);
        var url = $"{BaseUrl}/reference-data/locations/cities?keyword={encodedCity}&max=1";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("City lookup failed: " + content);

        var json = JsonDocument.Parse(content);
        var data = json.RootElement.GetProperty("data");

        if (data.GetArrayLength() == 0)
            throw new Exception("No city code found for the given city name.");

        var first = data[0];
        return first.GetProperty("iataCode").GetString();
    }

    public async Task<List<Hotel>> GetHotelsByCityCodeAsync(string cityCode)
    {
        await EnsureAuthenticatedAsync();

        var url = $"{BaseUrl}/reference-data/locations/hotels/by-city?cityCode={cityCode}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("Hotel lookup failed: " + content);

        var json = JsonDocument.Parse(content);
        var hotelsArray = json.RootElement.GetProperty("data");

        var hotels = new List<Hotel>();

        foreach (var item in hotelsArray.EnumerateArray())
        {
            var name = item.GetProperty("name").GetString();
            if (string.IsNullOrWhiteSpace(name) || name.Contains("AIRPORT", StringComparison.OrdinalIgnoreCase))
                continue;

            var hotel = new Hotel
            {   
                Name = name,
                HotelId = item.GetProperty("hotelId").GetString(),
                CityCode = cityCode,
                CountryCode = item.GetProperty("address").GetProperty("countryCode").GetString(),
                Latitude = item.GetProperty("geoCode").GetProperty("latitude").GetDouble(),
                Longitude = item.GetProperty("geoCode").GetProperty("longitude").GetDouble(),
                Price = random.Next(50, 121)
            };

            hotels.Add(hotel);
        }

        return hotels;
    }
}
