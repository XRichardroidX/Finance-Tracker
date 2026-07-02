using System.Text;
using FinanceTracker.Api.Data;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Services (dependency injection container) ---

// EF Core, pointed at Postgres via the connection string in appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Our own services, used by the controllers.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

// JWT authentication: every request with a valid Bearer token in the
// Authorization header gets a populated User (ClaimsPrincipal).
var jwtKey = builder.Configuration["Jwt:Key"]!;
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication(); // who are you?
app.UseAuthorization();  // are you allowed to do this?
app.MapControllers();

app.Run();
