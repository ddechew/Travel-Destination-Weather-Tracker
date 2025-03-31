using System.Net.Http.Json;
using System.Text.Json;
using DestinationsApp.Models;
using DestinationsApp.Services.Models;
using DestinationsApp.Utils;

public class CountryService
{
    private readonly HttpClient httpClient;

    public CountryService()
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://countriesnow.space/api/v0.1/")
        };
    }
        
    public async Task<List<string>> GetCountryNamesAsync()
    {
        var response = await httpClient.GetFromJsonAsync<CountryApiResponse>("countries"); 


        if (response != null && response.Data != null)
        {
            return response.Data
                .Select(c => c.ToString())
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        return new List<string>();
    }


    public async Task<List<string>> GetCitiesByCountryAsync(string countryName)
    {
        var requestBody = new { country = countryName };

        var response = await httpClient.PostAsJsonAsync("countries/cities", requestBody);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CityApiResponse>();

            if (result != null && result.Cities != null)
            {
                return result.Cities
                        .OrderBy(city => city)
                        .Where(city => !city.Contains("Obshtina"))
                        .ToList();
            }
        }

        return new List<string>();
    }
}
