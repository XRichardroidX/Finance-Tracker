# Personal Finance Tracker

A simple full-stack app for tracking personal expenses. Built as a learning project.

**Stack:** React (frontend) · ASP.NET Core Web API (backend) · PostgreSQL (database)

## Project files

- `PROJECT_PLAN.md` — full spec: every screen, every function, every endpoint, explained in detail.
- `CLAUDE.md` — rules for AI coding tools (Claude Code, Codex) working in this repo.
- `backend/` — ASP.NET Core Web API starter.
- `frontend/` — React (Vite) starter.

## Quick start

### Backend
```bash
cd backend/FinanceTracker.Api
dotnet restore
dotnet ef database update   # applies migrations to Postgres (see appsettings.json for connection string)
dotnet run
```
API runs at `http://localhost:5000`.

### Frontend
```bash
cd frontend
npm install
npm run dev
```
App runs at `http://localhost:5173`.

## Read this first if you're new to the codebase

Start with `PROJECT_PLAN.md`. It describes what the app does, screen by screen, before you look at any code. Then read the backend `Models/` folder (the data shapes), then `Controllers/` (the endpoints), then the frontend `pages/` folder in this order: `Login.jsx` → `Register.jsx` → `Dashboard.jsx` → `Expenses.jsx`. That's the same order a new user would move through the app.
