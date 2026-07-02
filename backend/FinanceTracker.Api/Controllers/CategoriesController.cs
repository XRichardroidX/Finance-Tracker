using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize] // every action in this controller requires a valid JWT
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/categories
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        var userId = User.GetUserId();

        var categories = await _db.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Color))
            .ToListAsync();

        return Ok(categories);
    }

    // POST /api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Category name is required." });

        var userId = User.GetUserId();

        var category = new Category
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Color = request.Color,
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return Ok(new CategoryResponse(category.Id, category.Name, category.Color));
    }
}
