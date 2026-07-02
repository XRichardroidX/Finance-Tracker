# Project Plan: Personal Finance Tracker

## 1. What this app does

A single user logs in and records their personal expenses. They can see a
monthly summary (total spent, spending by category, a chart) and a filterable
list of individual expenses. That's the whole app for v1 — no budgets, no
recurring transactions, no multi-currency, no shared accounts. Keeping the
scope small is intentional: every feature below should be fully built and
solid before anything is added.

## 2. Data model

Three tables. Every table (except `users`) has a `user_id` foreign key so
one user's data is never visible to another.

### `users`
| column        | type      | notes                        |
|---------------|-----------|-------------------------------|
| id            | uuid      | primary key                   |
| email         | text      | unique, not null              |
| password_hash | text      | never returned by any API     |
| created_at    | timestamp | default now()                 |

### `categories`
| column     | type      | notes                                  |
|------------|-----------|------------------------------------------|
| id         | uuid      | primary key                              |
| user_id    | uuid      | foreign key → users.id                   |
| name       | text      | e.g. "Groceries", "Rent"                 |
| color      | text      | hex string, used for chart segments      |

Every new user gets 6 default categories seeded on registration: Groceries,
Rent, Transport, Entertainment, Utilities, Other. Users can add more.

### `expenses`
| column      | type      | notes                                   |
|-------------|-----------|-------------------------------------------|
| id          | uuid      | primary key                                |
| user_id     | uuid      | foreign key → users.id                     |
| category_id | uuid      | foreign key → categories.id                |
| amount      | decimal   | positive number, 2 decimal places          |
| note        | text      | optional, e.g. "Trader Joe's"              |
| date        | date      | the date the expense happened (not created_at) |
| created_at  | timestamp | default now()                              |

## 3. Backend: API endpoints

All routes except `/api/auth/*` require a valid JWT in the
`Authorization: Bearer <token>` header. The backend reads the user's id from
the token — it is never taken from the request body, so one user can't query
another user's data by changing an id.

| Method | Route                        | Purpose                                             |
|--------|-------------------------------|------------------------------------------------------|
| POST   | `/api/auth/register`          | Create a user (email + password). Seeds default categories. Returns a JWT. |
| POST   | `/api/auth/login`              | Verify credentials, return a JWT.                    |
| GET    | `/api/categories`              | List the logged-in user's categories.                |
| POST   | `/api/categories`               | Create a new category.                                |
| GET    | `/api/expenses?month=YYYY-MM`  | List expenses for the given month (defaults to current month if omitted). |
| POST   | `/api/expenses`                 | Create an expense.                                    |
| PUT    | `/api/expenses/{id}`            | Update an expense (must belong to the logged-in user). |
| DELETE | `/api/expenses/{id}`            | Delete an expense (must belong to the logged-in user). |
| GET    | `/api/summary?month=YYYY-MM`    | Total spend + spend-per-category for that month.       |

**Validation rules (enforced server-side, in the Service layer, not the
controller):**
- `email` must be a valid email format and unique.
- `password` must be at least 8 characters.
- `amount` must be greater than 0.
- `date` cannot be in the future.
- `category_id` on an expense must belong to the same user.

## 4. Frontend: screens

Each screen is one file in `frontend/src/pages/`.

### `Login.jsx`
- Fields: email, password.
- Submit calls `POST /api/auth/login`.
- On success: store the JWT (in memory / React state via context — not
  localStorage, to reduce XSS token-theft risk for this learning project),
  redirect to Dashboard.
- On failure: show inline error "Invalid email or password" — never reveal
  whether the email exists.
- Link to Register.

### `Register.jsx`
- Fields: email, password, confirm password.
- Client-side check: passwords match, password ≥ 8 characters (real
  enforcement still happens on the backend).
- Submit calls `POST /api/auth/register`, then behaves like a successful
  login.

### `Dashboard.jsx`
- The default screen after login.
- Month selector (defaults to current month).
- Calls `GET /api/summary?month=...` and shows:
  - Total spent this month (large number, top of page).
  - A pie or bar chart of spend by category (Recharts).
  - A short list of the 5 most recent expenses, each linking to Expenses
    with that category pre-filtered.
- Empty state (no expenses yet this month): a message inviting the user to
  add their first expense, with a button that opens the add-expense form.

### `Expenses.jsx`
- Month selector + category filter dropdown.
- Calls `GET /api/expenses?month=...`, filters client-side by category if one
  is selected.
- Table/list of expenses: date, category (colored tag), note, amount, and
  edit/delete actions per row.
- "Add expense" button opens `ExpenseForm` (in a modal or inline panel).
- Deleting asks for confirmation before calling the DELETE endpoint.

## 5. Frontend: components

Each is one file in `frontend/src/components/`.

### `ExpenseForm.jsx`
- Fields: amount, category (dropdown from `GET /api/categories`), date
  (defaults to today), note (optional).
- Used for both creating and editing (an `expense` prop, if passed, pre-fills
  the form and switches the submit action from POST to PUT).
- Shows field-level validation errors returned from the API.

### `ExpenseList.jsx`
- Pure presentational component: takes an array of expenses + callbacks for
  edit/delete, renders the rows. Contains no fetch calls itself — all data
  comes from the page that renders it.

### `SummaryChart.jsx`
- Takes a `data` prop (array of `{ category, amount, color }`) and renders a
  Recharts pie chart. Contains no fetch calls.

## 6. Frontend: API layer

### `src/api/client.js`
- One place for every HTTP call. Exports functions like `login(email,
  password)`, `getExpenses(month)`, `createExpense(data)`, etc.
- Attaches the JWT to every request automatically (except auth endpoints).
- Centralizes error handling: on a 401 response, clears the stored token and
  redirects to Login.
- Pages never call `fetch` directly — they import from this file. This means
  if the API's base URL or auth scheme ever changes, it changes in one file.

## 7. What's deliberately out of scope for v1

- Multi-user sharing / family accounts
- Recurring or scheduled expenses
- Budgets or spending limits
- Multiple currencies
- Password reset flow (register a new account in dev/testing instead)
- Mobile app (responsive web is enough)

If you want to extend the project after v1 works end-to-end, these are the
natural next features — but build and test the core loop first.
