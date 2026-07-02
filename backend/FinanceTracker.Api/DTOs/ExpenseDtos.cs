namespace FinanceTracker.Api.DTOs;

public record ExpenseResponse(
    Guid Id,
    decimal Amount,
    string? Note,
    DateOnly Date,
    Guid CategoryId,
    string CategoryName,
    string CategoryColor
);

public record CreateExpenseRequest(decimal Amount, string? Note, DateOnly Date, Guid CategoryId);

public record UpdateExpenseRequest(decimal Amount, string? Note, DateOnly Date, Guid CategoryId);
