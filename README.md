# Personal Finance Tracker

A simple full-stack app for tracking personal expenses. Built as a learning project.

🔗 **[Live Demo](https://YOUR-VITE-APP.vercel.app)** *(Replace with your actual Vercel URL once deployed)*

**Stack:** React (frontend) · ASP.NET Core Web API (backend) · PostgreSQL (database)

## Project files

- `PROJECT_PLAN.md` — full spec: every screen, every function, every endpoint.
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

---

## Deployment

This app is configured to be deployed to **Railway** (Backend & Database) and **Vercel** (Frontend).

### 1. Database & Backend (Railway)
- Deploy your GitHub repository to Railway.
- Add a **PostgreSQL** database service in the same project.
- In your backend service variables, set the following:
  - `Jwt__Issuer`: `FinanceTrackerApi`
  - `Jwt__Audience`: `FinanceTrackerClient`
  - `Jwt__Key`: `<your-random-32-char-secret-key>`
  - `CORS_ORIGINS`: `https://YOUR-VITE-APP.vercel.app` (your frontend Vercel URL)
- Railway will automatically read `DATABASE_URL` and run database migrations on startup.

### 2. Frontend (Vercel)
- Deploy the repository to Vercel.
- Set the **Root Directory** of the project to `frontend`.
- Add an Environment Variable:
  - Key: `VITE_API_URL`
  - Value: `https://YOUR-BACKEND-API.railway.app` (exclude the trailing `/api`)
- Deploy!
