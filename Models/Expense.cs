using SQLite;

public class Expense
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Generic Expenses
    public string Type { get; set; } 
    public decimal Value { get; set; } 

    // Hotel-specific fields
    public string HotelName { get; set; }
    public decimal BookedPrice { get; set; }
    public int Duration { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public int DestinationId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    [Ignore]
    public bool IsHotel => Type?.ToLowerInvariant() == "hotel";

    [Ignore]
    public decimal TotalCost => BookedPrice * Duration;
}
