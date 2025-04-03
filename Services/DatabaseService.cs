namespace DestinationsApp.Services;

using DestinationsApp.Models;

using SQLite;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection database;

    public DatabaseService(string dbPath)
    {
        database = new SQLiteAsyncConnection(dbPath);
        database.CreateTableAsync<Destination>().Wait();
        database.CreateTableAsync<Expense>().Wait(); 

    }

    public Task<List<Destination>> GetDestinationsAsync()
    {
        return database.Table<Destination>().ToListAsync();
    }

    public Task<int> AddDestinationAsync(Destination destination)
    {
        return database.InsertAsync(destination);
    }

    public async Task<List<Expense>> GetExpensesByDestinationIdAsync(int destinationId)
    {
        return await database.Table<Expense>()
                              .Where(e => e.DestinationId == destinationId)
                              .ToListAsync();
    }


    public async Task<int> SaveExpenseAsync(Expense expense)
    {
        if (expense.Id != 0)
        {
            return await database.UpdateAsync(expense); 
        }
        else
        {
            return await database.InsertAsync(expense); 
        }
    }

    public Task<int> DeleteExpenseAsync(Expense expense)
    {
        return database.DeleteAsync(expense);
    }

    public async Task DeleteExpensesByDestinationIdAsync(int destinationId)
    {
        var expenses = await database.Table<Expense>().Where(e => e.DestinationId == destinationId).ToListAsync();
        foreach (var expense in expenses)
            await database.DeleteAsync(expense);
    }

    public Task<int> UpdateDestinationAsync(Destination destination)
    {
        return database.UpdateAsync(destination);
    }

    public Task<int> DeleteDestinationAsync(Destination destination)
    {
        return database.DeleteAsync(destination);
    }
}
