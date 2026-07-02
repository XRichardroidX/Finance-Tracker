namespace FinanceTracker.Api.DTOs;

// DTOs (Data Transfer Objects) define exactly what shape of data crosses the
// API boundary. We never send Models/*.cs entities directly to the client —
// that would risk leaking fields like PasswordHash.

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

// Returned after a successful register or login.
public record AuthResponse(string Token, string Email);
