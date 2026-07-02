namespace FinanceTracker.Api.Models;

// A spending category, e.g. "Groceries". Belongs to exactly one user —
// categories are never shared between users.
public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Hex color string (e.g. "#4C6EF5"), used to color chart segments and tags.
    public string Color { get; set; } = "#6B7280";

    public User? User { get; set; }
    public List<Expense> Expenses { get; set; } = new();
}
