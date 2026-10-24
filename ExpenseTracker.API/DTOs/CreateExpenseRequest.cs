using System.ComponentModel.DataAnnotations;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.DTOs;

public class CreateExpenseRequest
{
    [Required]
    [MinLength(8, ErrorMessage = "Description must be at least 8 characters long.")]
    [MaxLength(100, ErrorMessage = "Description must be max 100 characters long.")]

    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; }
}
