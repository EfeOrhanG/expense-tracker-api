namespace ExpenseTracker.API.Models;


public class Expense
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public ExpenseCategory Category { get; set; }

    public Guid UserId { get; set; }
    public User User {get; set;} = null!;
}