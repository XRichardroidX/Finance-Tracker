namespace FinanceTracker.Api.Models;

// A registered user. Never expose PasswordHash outside the Services layer —
// controllers should always return a DTO instead of this class directly.
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Category> Categories { get; set; } = new();
    public List<Expense> Expenses { get; set; } = new();
}
