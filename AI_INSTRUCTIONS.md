# Rules for AI coding tools (Claude Code, Codex, etc.)

This file is the contract. If you (the AI) are about to do something that isn't
covered here, stop and ask instead of guessing. This project is a learning
codebase — clarity and predictability matter more than cleverness.

## Do

- **Read `PROJECT_PLAN.md` before writing or changing any code.** It is the
  source of truth for what screens, endpoints, and fields exist.
- **Follow the existing folder structure exactly.**
  Backend: `Controllers/` → `Services/` → `Data/` (EF Core) → `Models/` (entities) → `DTOs/` (request/response shapes).
  Frontend: `pages/` (one file per screen) → `components/` (reusable pieces) → `api/` (all HTTP calls, nowhere else).
- **Keep controllers thin.** A controller method should: read the request →
  call a service → return a result. Business logic (validation, calculations,
  password hashing) belongs in `Services/`, not in the controller.
- **Use DTOs for every API request and response.** Never return an EF Core
  entity directly from a controller (it can leak fields like `PasswordHash`).
- **Use Entity Framework Core migrations for every schema change.**
  `dotnet ef migrations add <Name>` then `dotnet ef database update`. Never
  hand-edit the database schema directly.
- **Write a one-line comment above any function whose purpose isn't obvious
  from its name.** Assume the reader is a student seeing this pattern for the
  first time.
- **Match the naming conventions already in the code:** PascalCase for C#
  classes/methods, camelCase for C# local variables and JS, kebab-case for
  API routes (e.g. `/api/expenses`).
- **Ask before adding a new NuGet or npm package.** State why the existing
  packages can't do it.
- **Keep all HTTP calls on the frontend inside `src/api/client.js`.** Pages
  and components should call functions from there, never call `fetch`
  directly inline.
- **Validate input on the backend, even if the frontend already validates it.**
  The frontend check is for UX; the backend check is the real one.

## Don't

- **Don't invent endpoints, fields, or screens that aren't in `PROJECT_PLAN.md`.**
  If a feature seems missing, say so and ask — don't silently add it.
- **Don't hardcode secrets** (DB passwords, JWT signing keys) in source files.
  They belong in `appsettings.Development.json` (gitignored) or environment
  variables — never committed.
- **Don't change the authentication approach** (JWT in an `Authorization:
  Bearer` header) without being asked. Don't switch to cookies, sessions, or
  a third-party auth provider on your own initiative.
- **Don't write raw SQL strings.** Use EF Core's LINQ query syntax. If a query
  genuinely needs raw SQL, ask first and explain why LINQ can't express it.
- **Don't "helpfully" refactor unrelated code while completing a task.** If
  you notice something that should be improved, mention it separately instead
  of changing it inline — small, reviewable diffs only.
- **Don't guess at a library's API.** If you're not certain a method exists
  or behaves a certain way, say so rather than presenting it as fact. This
  codebase intentionally uses only well-documented, mainstream packages
  (EF Core, ASP.NET Core Identity-style hashing, React Router, Recharts) —
  check the plan or ask rather than inventing usage.
- **Don't remove the comments that explain "why"** when editing a function,
  even if you rewrite the "how."
- **Don't skip error handling** on API calls. Every frontend request should
  handle both the success and failure case (e.g. show an error message, not
  a silent failure or an unhandled promise rejection).
- **Don't combine the Categories and Expenses tables, or otherwise change the
  schema in `PROJECT_PLAN.md`,** without flagging the change and the reason.

## When unsure

State the assumption you're about to make and why, in one sentence, before
writing the code — not after. If the ambiguity is significant (affects the
schema, auth, or a whole screen), ask instead of assuming.
