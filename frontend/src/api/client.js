// This file is the ONLY place that calls fetch() in the whole app.
// Pages and components import functions from here instead of calling
// fetch directly - see CLAUDE.md for why.

// VITE_API_URL is set at build time (e.g. on Vercel) to point to the deployed backend.
// Falls back to localhost for local development.
const API_BASE = (import.meta.env.VITE_API_URL ?? 'http://localhost:5000') + '/api'

// The JWT lives in memory (a module-level variable), not localStorage.
// This means it's cleared on a page refresh - acceptable for a learning
// project, and it avoids the token being readable by any injected script
// (a common XSS risk with localStorage). AuthContext calls setToken()
// after login/register.
let token = null
export function setToken(newToken) {
  token = newToken
}

// Handles every request: attaches the auth header, parses JSON, and
// turns non-2xx responses into a thrown error with the server's message.
async function request(path, { method = 'GET', body } = {}) {
  const headers = { 'Content-Type': 'application/json' }
  if (token) headers['Authorization'] = `Bearer ${token}`

  const res = await fetch(`${API_BASE}${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  })

  if (res.status === 401) {
    setToken(null)
    window.location.href = '/login'
    throw new Error('Session expired. Please log in again.')
  }

  const data = res.status === 204 ? null : await res.json().catch(() => null)

  if (!res.ok) {
    throw new Error(data?.message || 'Something went wrong. Please try again.')
  }

  return data
}

// --- Auth ---
export const login = (email, password) =>
  request('/auth/login', { method: 'POST', body: { email, password } })

export const register = (email, password) =>
  request('/auth/register', { method: 'POST', body: { email, password } })

// --- Categories ---
export const getCategories = () => request('/categories')

export const createCategory = (name, color) =>
  request('/categories', { method: 'POST', body: { name, color } })

// --- Expenses ---
export const getExpenses = (month) => request(`/expenses?month=${month}`)

export const createExpense = (expense) =>
  request('/expenses', { method: 'POST', body: expense })

export const updateExpense = (id, expense) =>
  request(`/expenses/${id}`, { method: 'PUT', body: expense })

export const deleteExpense = (id) =>
  request(`/expenses/${id}`, { method: 'DELETE' })

// --- Summary ---
export const getSummary = (month) => request(`/summary?month=${month}`)
