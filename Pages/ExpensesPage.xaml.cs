namespace DestinationsApp;

using DestinationsApp.Models;

public partial class ExpensesPage : ContentPage
{
    private Destination _destination;

    public ExpensesPage(Destination existingDestination)
    {
        InitializeComponent();
        _destination = existingDestination;

        LoadExpensesAsync();
    }

    private async void OnSaveExpenseClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TypeEntry.Text) || string.IsNullOrWhiteSpace(ValueEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in both Type and Value.", "OK");
            return;
        }

        if (!decimal.TryParse(ValueEntry.Text, out decimal value))
        {
            await DisplayAlert("Error", "Value must be a valid number.", "OK");
            return;
        }

        var genericExpense = new Expense
        {
            Type = TypeEntry.Text,
            Value = value,
            DestinationId = _destination.Id
        };

        await App.Database.SaveExpenseAsync(genericExpense);
        await LoadExpensesAsync();
        TypeEntry.Text = string.Empty;
        ValueEntry.Text = string.Empty;
    }

    private async Task LoadExpensesAsync()
    {
        var expenses = await App.Database.GetExpensesByDestinationIdAsync(_destination.Id);
        ExpensesListView.ItemsSource = expenses;

        decimal total = 0;

        foreach (var e in expenses)
        {
            if (e.IsHotel)
                total += e.TotalCost;
            else
                total += e.Value;
        }

        TotalLabel.Text = $"Total Expenses: {total} BGN";
    }

    private async void OnDeleteExpenseClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Expense expenseToDelete)
        {
            bool confirm = await DisplayAlert("Confirm", $"Delete expense for '{expenseToDelete.HotelName}'?", "Yes", "No");

            if (confirm)
            {
                await App.Database.DeleteExpenseAsync(expenseToDelete);

                var updatedList = await App.Database.GetExpensesByDestinationIdAsync(_destination.Id);
                ExpensesListView.ItemsSource = updatedList;
            }
        }
    }

    private async void OnChooseHotelClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HotelsPage(_destination));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadExpensesAsync();
    }

    private async void OnViewHotelClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Expense expense)
        {
            var hotel = new Hotel
            {
                HotelId = expense.HotelName, 
                Name = expense.HotelName,
                CityCode = expense.City,
                CountryCode = expense.Country,
                Latitude = expense.Latitude,
                Longitude = expense.Longitude,
                Price = expense.BookedPrice 
            };

            await Navigation.PushAsync(new HotelDetailsPage(hotel, _destination));
        }
    }

    private async void OnEditExpenseClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Expense expense)
        {
            if (!expense.IsHotel)
            {
                string valueInput = await DisplayPromptAsync(
                    "Edit Value",
                    $"Current: {expense.Value} BGN\nEnter new value:",
                    initialValue: expense.Value.ToString(),
                    keyboard: Keyboard.Numeric
                );

                if (decimal.TryParse(valueInput, out decimal newValue) && newValue >= 0)
                {
                    expense.Value = newValue;
                    await App.Database.SaveExpenseAsync(expense);
                    await LoadExpensesAsync();
                }
            }
            else
            {
                await DisplayAlert("Info", "Hotel expenses can only be edited from the Hotel Details page.", "OK");
            }
        }
    }
}