import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { login } from '../api/client'
import { useAuth } from '../context/AuthContext'

export default function Login() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState(null)
  const [loading, setLoading] = useState(false)

  const { signIn } = useAuth()
  const navigate = useNavigate()

  async function handleSubmit(e) {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const result = await login(email, password)
      signIn(result)
      navigate('/')
    } catch (err) {
      // The backend deliberately returns a vague message here
      // ("Invalid email or password") - we just show it as-is.
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-page">
      <div className="card stack">
        <div>
          <h1>Welcome back</h1>
          <p className="muted">Log in to see your expenses.</p>
        </div>

        <form className="stack" onSubmit={handleSubmit}>
          <div>
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div>
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          {error && <p className="error-text">{error}</p>}

          <button className="btn-primary" type="submit" disabled={loading}>
            {loading ? 'Logging in…' : 'Log in'}
          </button>
        </form>

        <p className="muted">
          No account yet? <Link className="btn-link" to="/register">Register</Link>
        </p>
      </div>
    </div>
  )
}
