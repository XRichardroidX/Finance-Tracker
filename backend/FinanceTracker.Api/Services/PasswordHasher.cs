using System.Security.Cryptography;

namespace FinanceTracker.Api.Services;

// Hashes and verifies passwords using PBKDF2 (industry-standard, built into
// .NET — no extra package needed). Never store or log plain-text passwords.
public static class PasswordHasher
{
    private const int SaltSize = 16;      // bytes
    private const int HashSize = 32;      // bytes
    private const int Iterations = 100_000;

    // Produces a string of the form "iterations.saltBase64.hashBase64"
    // so the hash is self-describing and can be verified later even if
    // the iteration count changes in the future.
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3) return false;

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = Convert.FromBase64String(parts[2]);

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
