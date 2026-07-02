using System.Text;
using FinanceTracker.Api.Data;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Services (dependency injection container) ---

// EF Core, pointed at Postgres via the connection string in appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If DATABASE_URL is set (e.g. on Railway/Heroku), parse it into Npgsql format
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl) && (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";
    var host = uri.Host;
    var port = uri.Port;
    var database = uri.AbsolutePath.TrimStart('/');
    
    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Our own services, used by the controllers.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

// JWT authentication: every request with a valid Bearer token in the
// Authorization header gets a populated User (ClaimsPrincipal).
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    // A fallback key to prevent startup crashes when the environment variable is not yet configured.
    jwtKey = "super_secret_key_for_development_and_testing_only_must_be_changed_in_production_32_bytes";
}
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();

// Allow the React dev server (and later, the deployed frontend) to call this API.
// CORS_ORIGINS env var is a comma-separated list of allowed origins set in production.
// Falls back to localhost for local development if the env var is not set.
const string CorsPolicy = "AllowFrontend";
var allowedOrigins = (builder.Configuration["CORS_ORIGINS"] ?? "http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Automatically apply any pending EF Core migrations on startup.
// This means Railway (and any other host) never needs a manual "dotnet ef database update" step.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// --- Middleware pipeline (order matters here) ---

// Enable Swagger in all environments (including production) so that
// testing via the web UI is possible and the Railway health check passes.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication(); // who are you?
app.UseAuthorization();  // are you allowed to do this?
app.MapControllers();

app.Run();
