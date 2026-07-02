using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/summary")]
[Authorize]
public class SummaryController : ControllerBase
{
    private readonly AppDbContext _db;

    public SummaryController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/summary?month=2026-07
    [HttpGet]
    public async Task<ActionResult<MonthlySummaryResponse>> Get([FromQuery] string? month)
    {
        var userId = User.GetUserId();

        DateOnly start;
        string monthLabel;
        if (month is not null && DateOnly.TryParseExact(month + "-01", "yyyy-MM-dd", out var parsed))
        {
            start = parsed;
            monthLabel = month;
        }
        else
        {
            var now = DateTime.UtcNow;
            start = new DateOnly(now.Year, now.Month, 1);
            monthLabel = start.ToString("yyyy-MM");
        }
        var end = start.AddMonths(1);

        var expenses = await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.Date >= start && e.Date < end)
            .ToListAsync();

        var byCategory = expenses
            .GroupBy(e => new { e.Category!.Name, e.Category.Color })
            .Select(g => new CategoryTotal(g.Key.Name, g.Key.Color, g.Sum(e => e.Amount)))
            .OrderByDescending(c => c.Total)
            .ToList();

        var response = new MonthlySummaryResponse(
            Month: monthLabel,
            Total: expenses.Sum(e => e.Amount),
            ByCategory: byCategory
        );

        return Ok(response);
    }
}
