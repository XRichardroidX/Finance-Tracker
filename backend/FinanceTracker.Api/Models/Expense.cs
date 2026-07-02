namespace FinanceTracker.Api.Models;

// A single recorded expense.
public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }
    public string? Note { get; set; }

    // The date the money was actually spent (distinct from CreatedAt,
    // which is when the record was entered into the system).
    public DateOnly Date { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Category? Category { get; set; }
}
