import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getExpenses, getCategories, deleteExpense } from '../api/client'
import ExpenseList from '../components/ExpenseList'
import ExpenseForm from '../components/ExpenseForm'

function currentMonth() {
  return new Date().toISOString().slice(0, 7)
}

export default function Expenses() {
  const [month, setMonth] = useState(currentMonth())
  const [categoryFilter, setCategoryFilter] = useState('')
  const [expenses, setExpenses] = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  // Controls the add/edit modal: null = closed, {} = adding, {...} = editing.
  const [editingExpense, setEditingExpense] = useState(null)
  const [formOpen, setFormOpen] = useState(false)

  async function reload() {
    setLoading(true)
    setError(null)
    try {
      const [expensesData, categoriesData] = await Promise.all([
        getExpenses(month),
        getCategories(),
      ])
      setExpenses(expensesData)
      setCategories(categoriesData)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { reload() }, [month])

  const visibleExpenses = categoryFilter
    ? expenses.filter((e) => e.categoryId === categoryFilter)
    : expenses

  function openAddForm() {
    setEditingExpense(null)
    setFormOpen(true)
  }

  function openEditForm(expense) {
    setEditingExpense(expense)
    setFormOpen(true)
  }

  async function handleDelete(id) {
    if (!confirm('Delete this expense?')) return
    try {
      await deleteExpense(id)
      reload()
    } catch (err) {
      setError(err.message)
    }
  }

  function handleFormDone() {
    setFormOpen(false)
    reload()
  }

  return (
    <div className="page stack">
      <div className="row-between">
        <h1>Expenses</h1>
        <Link className="btn-link" to="/">Back to dashboard</Link>
      </div>

      <div className="row">
        <input
          type="month"
          value={month}
          onChange={(e) => setMonth(e.target.value)}
          style={{ width: 160 }}
        />
        <select value={categoryFilter} onChange={(e) => setCategoryFilter(e.target.value)}>
          <option value="">All categories</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
        <button className="btn-primary" onClick={openAddForm}>Add expense</button>
      </div>

      {error && <p className="error-text">{error}</p>}

      <div className="card">
        {loading ? (
          <p className="muted">Loading…</p>
        ) : (
          <ExpenseList expenses={visibleExpenses} onEdit={openEditForm} onDelete={handleDelete} />
        )}
      </div>

      {formOpen && (
        <ExpenseForm
          expense={editingExpense}
          categories={categories}
          onDone={handleFormDone}
          onCancel={() => setFormOpen(false)}
        />
      )}
    </div>
  )
}
