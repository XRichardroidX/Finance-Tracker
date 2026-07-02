using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    // Seeded for every new user so the app isn't empty on first login.
    // Matches the list in PROJECT_PLAN.md — keep them in sync if you change one.
    private static readonly (string Name, string Color)[] DefaultCategories =
    {
        ("Groceries", "#4C6EF5"),
        ("Rent", "#F76707"),
        ("Transport", "#37B24D"),
        ("Entertainment", "#AE3EC9"),
        ("Utilities", "#F59F00"),
        ("Other", "#868E96"),
    };

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (!IsValidEmail(request.Email))
            throw new ArgumentException("Enter a valid email address.");

        if (request.Password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.");

        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
            throw new ArgumentException("An account with this email already exists.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = PasswordHasher.Hash(request.Password),
        };
        _db.Users.Add(user);

        foreach (var (name, color) in DefaultCategories)
        {
            _db.Categories.Add(new Category { UserId = user.Id, Name = name, Color = color });
        }

        await _db.SaveChangesAsync();

        return new AuthResponse(_jwt.GenerateToken(user), user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Deliberately vague error — don't reveal whether the email exists.
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthResponse(_jwt.GenerateToken(user), user.Email);
    }

    private static bool IsValidEmail(string email)
    {
        try { _ = new System.Net.Mail.MailAddress(email); return true; }
        catch { return false; }
    }
}
