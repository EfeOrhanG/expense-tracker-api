using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTOs;
using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.API.Models;
using System.Security.Claims;


namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpenseController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public IActionResult CreateExpense([FromBody] CreateExpenseRequest request)
    {
        string description = request.Description;
        decimal amount = request.Amount;
        ExpenseCategory expenseCategory = request.Category;
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var expense = new Expense
        {
            Description = description,
            Amount = amount,
            Category = expenseCategory,
            UserId = userId,
            ExpenseDate = DateTime.UtcNow
        };
        _context.Expenses.Add(expense);
        _context.SaveChanges();
        
        // Implement the logic to create an expense
        return Ok(new { Message = "Expense created successfully." , 
            Expense = expense
        });
    }

    [HttpGet("list")]
    [Authorize]
    public IActionResult GetExpenses()
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var expenses = _context.Expenses
        .Where(x => x.UserId == userId).ToList();
        // Implement the logic to retrieve expenses
        return Ok(new { Message = "Expenses retrieved successfully.",
            Result = expenses
         });
     
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetExpenseById(Guid id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var expense = _context.Expenses
            .FirstOrDefault(x => x.Id == id && x.UserId == userId);

        if (expense is null)
        {
            return NotFound(new { Message = $"Expense with ID {id} was not found." });
        }

        return Ok(new
        {
            Message = $"Expense with ID {id} retrieved successfully.",
            Result = expense
        });
    }


    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteExpense(Guid id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var expense = _context.Expenses
            .FirstOrDefault(x => x.Id == id && x.UserId == userId);

        if (expense is null)
        {
            return NotFound(new { Message = $"Expense with ID {id} was not found." });
        }

        _context.Expenses.Remove(expense);
        _context.SaveChanges();

        return Ok(new { Message = $"Expense with ID {id} deleted successfully." });
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var expense = _context.Expenses
            .FirstOrDefault(x => x.Id == id && x.UserId == userId);

        if (expense is null)
        {
            return NotFound(new { Message = $"Expense with ID {id} was not found." });
        }

        if (request.Description is not null)
        {
            expense.Description = request.Description;
        }

        if (request.Amount.HasValue)
        {
            expense.Amount = request.Amount.Value;
        }

        if (request.Date.HasValue)
        {
            expense.ExpenseDate = request.Date.Value;
        }

        _context.SaveChanges();

        return Ok(new
        {
            Message = $"Expense with ID {id} updated successfully.",
            Expense = expense
        });
    }

    [HttpGet("period")]
    [Authorize]
    public IActionResult GetExpensesByPeriod([FromQuery] string period)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var startDate = period.Trim().ToLowerInvariant() switch
        {
            "last-week" => now.AddDays(-7),
            "last-month" => now.AddMonths(-1),
            "last-3-months" => now.AddMonths(-3),
            _ => (DateTime?)null
        };

        if (startDate is null)
        {
            return BadRequest(new
            {
                Message = "Period must be last-week, last-month, or last-3-months."
            });
        }

        var expenses = _context.Expenses
            .Where(x => x.UserId == userId &&
                        x.ExpenseDate >= startDate.Value &&
                        x.ExpenseDate <= now)
            .ToList();

        return Ok(new
        {
            Message = $"Expenses for period '{period}' retrieved successfully.",
            Result = expenses
        });
    }

    private bool TryGetUserId(out Guid userId)
    {
        return Guid.TryParse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out userId);
    }

}
