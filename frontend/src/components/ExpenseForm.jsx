import { useEffect, useState } from 'react'
import { createExpense, updateExpense } from '../api/client'

// If `expense` is passed in, the form pre-fills and switches to "edit mode"
// (PUT instead of POST). Same component either way - one place that knows
// how to build an expense.
export default function ExpenseForm({ expense, categories, onDone, onCancel }) {
  const isEditing = Boolean(expense)

  const [amount, setAmount] = useState(expense?.amount ?? '')
  const [categoryId, setCategoryId] = useState(expense?.categoryId ?? categories[0]?.id ?? '')
  const [date, setDate] = useState(expense?.date ?? new Date().toISOString().slice(0, 10))
  const [note, setNote] = useState(expense?.note ?? '')
  const [error, setError] = useState(null)
  const [saving, setSaving] = useState(false)

  // If categories load after the form first renders, default to the first one.
  useEffect(() => {
    if (!categoryId && categories.length > 0) setCategoryId(categories[0].id)
  }, [categories, categoryId])

  async function handleSubmit(e) {
    e.preventDefault()
    setError(null)
    setSaving(true)

    const payload = { amount: Number(amount), note: note || null, date, categoryId }

    try {
      if (isEditing) {
        await updateExpense(expense.id, payload)
      } else {
        await createExpense(payload)
      }
      onDone()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="modal-backdrop" onClick={onCancel}>
      <form className="modal stack" onClick={(e) => e.stopPropagation()} onSubmit={handleSubmit}>
        <h2>{isEditing ? 'Edit expense' : 'Add expense'}</h2>

        <div>
          <label htmlFor="amount">Amount</label>
          <input
            id="amount"
            type="number"
            step="0.01"
            min="0.01"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            required
          />
        </div>

        <div>
          <label htmlFor="category">Category</label>
          <select id="category" value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="date">Date</label>
          <input
            id="date"
            type="date"
            value={date}
            max={new Date().toISOString().slice(0, 10)}
            onChange={(e) => setDate(e.target.value)}
            required
          />
        </div>

        <div>
          <label htmlFor="note">Note (optional)</label>
          <input id="note" type="text" value={note} onChange={(e) => setNote(e.target.value)} />
        </div>

        {error && <p className="error-text">{error}</p>}

        <div className="row">
          <button className="btn-primary" type="submit" disabled={saving}>
            {saving ? 'Saving…' : 'Save'}
          </button>
          <button className="btn-secondary" type="button" onClick={onCancel}>Cancel</button>
        </div>
      </form>
    </div>
  )
}
