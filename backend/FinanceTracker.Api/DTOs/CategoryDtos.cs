namespace FinanceTracker.Api.DTOs;

public record CategoryResponse(Guid Id, string Name, string Color);

public record CreateCategoryRequest(string Name, string Color);
