using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/expenses?month=2026-07  (month is optional, defaults to current month)
    [HttpGet]
    public async Task<ActionResult<List<ExpenseResponse>>> GetAll([FromQuery] string? month)
    {
        var userId = User.GetUserId();
        var (start, end) = ParseMonthRange(month);

        var expenses = await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.Date >= start && e.Date < end)
            .OrderByDescending(e => e.Date)
            .Select(e => new ExpenseResponse(
                e.Id, e.Amount, e.Note, e.Date,
                e.CategoryId, e.Category!.Name, e.Category.Color))
            .ToListAsync();

        return Ok(expenses);
    }

    // POST /api/expenses
    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create(CreateExpenseRequest request)
    {
        var userId = User.GetUserId();

        var error = await ValidateExpenseAsync(request.Amount, request.Date, request.CategoryId, userId);
        if (error is not null) return BadRequest(new { message = error });

        var expense = new Expense
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Note = request.Note,
            Date = request.Date,
        };
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return Ok(await ToResponseAsync(expense.Id));
    }

    // PUT /api/expenses/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseResponse>> Update(Guid id, UpdateExpenseRequest request)
    {
        var userId = User.GetUserId();

        // Scoping by UserId here (not just Id) is what stops user A from
        // editing user B's expense by guessing an id.
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense is null) return NotFound();

        var error = await ValidateExpenseAsync(request.Amount, request.Date, request.CategoryId, userId);
        if (error is not null) return BadRequest(new { message = error });

        expense.Amount = request.Amount;
        expense.Note = request.Note;
        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;
        await _db.SaveChangesAsync();

        return Ok(await ToResponseAsync(expense.Id));
    }

    // DELETE /api/expenses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();

        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense is null) return NotFound();

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // --- helpers ---

    private async Task<string?> ValidateExpenseAsync(decimal amount, DateOnly date, Guid categoryId, Guid userId)
    {
        if (amount <= 0) return "Amount must be greater than 0.";
        if (date > DateOnly.FromDateTime(DateTime.UtcNow)) return "Date cannot be in the future.";

        var categoryBelongsToUser = await _db.Categories.AnyAsync(c => c.Id == categoryId && c.UserId == userId);
        if (!categoryBelongsToUser) return "Category not found.";

        return null;
    }

    private async Task<ExpenseResponse> ToResponseAsync(Guid expenseId)
    {
        return await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.Id == expenseId)
            .Select(e => new ExpenseResponse(
                e.Id, e.Amount, e.Note, e.Date,
                e.CategoryId, e.Category!.Name, e.Category.Color))
            .FirstAsync();
    }

    // Parses "2026-07" into [2026-07-01, 2026-08-01). Falls back to the
    // current month if no value (or a malformed one) is provided.
    private static (DateOnly start, DateOnly end) ParseMonthRange(string? month)
    {
        DateOnly start;
        if (month is not null && DateOnly.TryParseExact(month + "-01", "yyyy-MM-dd", out var parsed))
        {
            start = parsed;
        }
        else
        {
            var now = DateTime.UtcNow;
            start = new DateOnly(now.Year, now.Month, 1);
        }

        var end = start.AddMonths(1);
        return (start, end);
    }
}
