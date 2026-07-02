// Pure presentational component: renders rows and delegates actions
// upward via callbacks. It has no idea how edit/delete are implemented -
// that keeps it reusable and easy to test.
export default function ExpenseList({ expenses, onEdit, onDelete }) {
  if (expenses.length === 0) {
    return <p className="muted">No expenses for this month yet.</p>
  }

  return (
    <div>
      {expenses.map((expense) => (
        <div className="expense-row" key={expense.id}>
          <span className="muted">{expense.date}</span>

          <div>
            <span className="tag" style={{ background: expense.categoryColor }}>
              {expense.categoryName}
            </span>
            {expense.note && <span className="muted"> {expense.note}</span>}
          </div>

          <strong>\u20a6{expense.amount.toFixed(2)}</strong>

          <div className="row">
            <button className="btn-link" onClick={() => onEdit(expense)}>Edit</button>
            <button className="btn-danger" onClick={() => onDelete(expense.id)}>Delete</button>
          </div>
        </div>
      ))}
    </div>
  )
}
