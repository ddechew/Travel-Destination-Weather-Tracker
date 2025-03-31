# Travel Destination & Weather Tracker

A cross-platform .NET MAUI mobile application that allows users to:

- Track travel destinations
- Record and view expenses (including hotel-specific and generic)
- View detailed weather forecasts for selected cities

---

## ✨ Features

### 📍 Destinations Management
- Add, edit, or delete travel destinations
- Set status: `Planned`, `Ongoing`, `Completed`, or `Cancelled`
- Start and end dates
- Duration, rating, and travel purpose
- Cities dynamically fetched based on selected country

### 🌎 Weather Forecasts
- Fetch 7-day forecasts via [WeatherAPI.com](https://www.weatherapi.com/)
- View:
  - Current weather
  - Hourly forecast with icons and temperature
  - 3-day forecast with min/max temperatures
  - Wind speed, humidity, and rain chance

### 💰 Expense Tracking
- Add generic expenses by type and value
- Add hotel expenses from Amadeus hotel API:
  - Hotel name, price per night, duration, total
  - Edit hotel booking duration only via hotel details
- View total expenses per destination

---

## 💡 Technologies Used

- [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/) (Multi-platform App UI)
- [SQLite](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/) local database
- [Amadeus API](https://developers.amadeus.com/) for hotel listings
- [WeatherAPI](https://www.weatherapi.com/) for weather data
- XAML & C# for UI and business logic

---

## 🚫 Sensitive Data Handling

All API keys are stored securely in `appsettings.json`, which is excluded from version control using `.gitignore`.

### Sample `appsettings.json`
```json
{
  "Amadeus": {
    "ClientId": "YOUR_AMADEUS_CLIENT_ID",
    "ClientSecret": "YOUR_AMADEUS_CLIENT_SECRET"
  },
  "WeatherApiKey": "YOUR_WEATHER_API_KEY"
}
```

> Don't forget to update your `ConfigurationService.cs` to read from this file at runtime.

---

## 💪 Key Architectural Decisions
- **Shell navigation** was used to simplify page routing
- `NavigationContext.cs` stores temporary navigation data like destinations, country lists, etc.
- MVU was initially considered but MVVM was adopted for flexibility

---

## 📓 Future Improvements
- Authentication (Firebase or Azure AD B2C)
- Cloud sync for destinations and expenses
- More advanced filtering and reporting
- Hotel booking integration

---

## 📁 Folder Structure
```
- Models/           // Domain models like Destination, Expense, Hotel
- Services/         // API & database services (Weather, Amadeus, SQLite)
- Pages/            // XAML pages for Destinations, Hotels, Weather, Expenses
- Utils/            // Shared utilities and navigation context
- appsettings.json  // API Keys (excluded from git)
```

---

## 🚀 Getting Started
1. Clone this repo
2. Create your own `appsettings.json` in the project root
3. Add your Amadeus and Weather API credentials
4. Run the app via Visual Studio

---

## 👀 Screenshots
*Coming soon...*

---

## ✍️ Author - Decho Dechev
Built with determination, iterations, and late nights ☕️
