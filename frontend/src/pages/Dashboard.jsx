import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getSummary, getExpenses } from '../api/client'
import { useAuth } from '../context/AuthContext'
import SummaryChart from '../components/SummaryChart'

function currentMonth() {
  return new Date().toISOString().slice(0, 7) // "2026-07"
}

export default function Dashboard() {
  const [month, setMonth] = useState(currentMonth())
  const [summary, setSummary] = useState(null)
  const [recent, setRecent] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const { user, signOut } = useAuth()

  // Reload whenever the selected month changes.
  useEffect(() => {
    let cancelled = false

    async function load() {
      setLoading(true)
      setError(null)
      try {
        const [summaryData, expensesData] = await Promise.all([
          getSummary(month),
          getExpenses(month),
        ])
        if (cancelled) return
        setSummary(summaryData)
        setRecent(expensesData.slice(0, 5))
      } catch (err) {
        if (!cancelled) setError(err.message)
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => { cancelled = true }
  }, [month])

  return (
    <div className="page stack">
      <div className="row-between">
        <div>
          <h1>Dashboard</h1>
          <p className="muted">{user?.email}</p>
        </div>
        <div className="row">
          <Link className="btn-link" to="/expenses">All expenses</Link>
          <button className="btn-secondary" onClick={signOut}>Log out</button>
        </div>
      </div>

      <input
        type="month"
        value={month}
        onChange={(e) => setMonth(e.target.value)}
        style={{ width: 160 }}
      />

      {error && <p className="error-text">{error}</p>}

      {loading ? (
        <p className="muted">Loading…</p>
      ) : summary.total === 0 ? (
        // Empty state: no expenses recorded for this month yet.
        <div className="card stack">
          <p>No expenses recorded for this month yet.</p>
          <Link className="btn-primary" to="/expenses" style={{ textDecoration: 'none', display: 'inline-block', textAlign: 'center' }}>
            Add your first expense
          </Link>
        </div>
      ) : (
        <>
          <div className="card">
            <p className="muted">Total spent</p>
            <p className="total-figure">₦{summary.total.toFixed(2)}</p>
          </div>

          <div className="card">
            <h2>By category</h2>
            <SummaryChart data={summary.byCategory} />
          </div>

          <div className="card">
            <h2>Recent expenses</h2>
            {recent.map((e) => (
              <div className="expense-row" key={e.id} style={{ gridTemplateColumns: '90px 1fr auto' }}>
                <span className="muted">{e.date}</span>
                <span className="tag" style={{ background: e.categoryColor }}>{e.categoryName}</span>
                <strong>₦{e.amount.toFixed(2)}</strong>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  )
}
