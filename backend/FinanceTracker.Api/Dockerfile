# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first (better Docker layer caching).
COPY FinanceTracker.Api.csproj .
RUN dotnet restore

# Copy the rest of the source and publish a release build.
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Railway injects PORT; ASP.NET Core reads ASPNETCORE_URLS.
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "FinanceTracker.Api.dll"]
