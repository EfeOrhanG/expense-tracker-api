namespace ExpenseTracker.API.DTOs;

public class UpdateExpenseRequest
{
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }
}