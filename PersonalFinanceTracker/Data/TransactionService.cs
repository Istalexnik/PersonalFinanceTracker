using System.Text.Json;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Data;

public class TransactionService
{
    private readonly string filePath;
    private List<Transaction> transactions;

    public TransactionService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PersonalFinanceTracker");
        Directory.CreateDirectory(appDataPath); // Ensures the directory exists
        filePath = Path.Combine(appDataPath, "transactions.json");
        transactions = LoadTransactions();
    }

    public IEnumerable<Transaction> GetTransactions() => transactions;

    public void AddTransaction(Transaction transaction)
    {
        transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;
        transactions.Add(transaction);
        SaveTransactions();
    }

    public IEnumerable<Transaction> GetTransactionsByType(string type)
    {
        return transactions.Where(t => t.Type == type);
    }

    public decimal GetTotalIncome()
    {
        return transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
    }

    public decimal GetTotalExpenses()
    {
        return transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
    }

    private void SaveTransactions()
    {
        var json = JsonSerializer.Serialize(transactions);
        File.WriteAllText(filePath, json);
    }

    private List<Transaction> LoadTransactions()
    {
        if (!File.Exists(filePath))
            return new List<Transaction>();

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
    }
}
