using System;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTOs;
using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.API.Models;
using System.Security.Claims;


namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _tokenService;

    public ExpenseController(AppDbContext context, JwtService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
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
        // Implement the logic to retrieve an expense by ID
        return Ok(new { Message = $"Expense with ID {id} retrieved successfully." });
    }


    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteExpense(Guid id)
    {
        // Implement the logic to delete an expense
        return Ok(new { Message = $"Expense with ID {id} deleted successfully." });
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        // Implement the logic to update an expense
        return Ok(new { Message = $"Expense with ID {id} updated successfully." });
    }

    public IActionResult GetExpensesByPeriod(string period)
    {
        // Implement the logic to retrieve expenses by period
        return Ok(new { Message = $"Expenses for period '{period}' retrieved successfully." });
    }


}
