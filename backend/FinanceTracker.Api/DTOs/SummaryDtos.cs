namespace FinanceTracker.Api.DTOs;

public record CategoryTotal(string CategoryName, string CategoryColor, decimal Total);

public record MonthlySummaryResponse(string Month, decimal Total, List<CategoryTotal> ByCategory);
