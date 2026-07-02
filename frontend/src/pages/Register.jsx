import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../api/client'
import { useAuth } from '../context/AuthContext'

export default function Register() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState(null)
  const [loading, setLoading] = useState(false)

  const { signIn } = useAuth()
  const navigate = useNavigate()

  // Client-side checks are for quick feedback only - the backend enforces
  // the real rules (see AuthService.RegisterAsync) and returns its own
  // error message if something slips through here.
  function validate() {
    if (password.length < 8) return 'Password must be at least 8 characters.'
    if (password !== confirmPassword) return 'Passwords do not match.'
    return null
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setError(null)

    const clientError = validate()
    if (clientError) {
      setError(clientError)
      return
    }

    setLoading(true)
    try {
      const result = await register(email, password)
      signIn(result)
      navigate('/')
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-page">
      <div className="card stack">
        <div>
          <h1>Create an account</h1>
          <p className="muted">We'll set you up with default spending categories.</p>
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
          <div>
            <label htmlFor="confirmPassword">Confirm password</label>
            <input
              id="confirmPassword"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          </div>

          {error && <p className="error-text">{error}</p>}

          <button className="btn-primary" type="submit" disabled={loading}>
            {loading ? 'Creating account…' : 'Create account'}
          </button>
        </form>

        <p className="muted">
          Already have an account? <Link className="btn-link" to="/login">Log in</Link>
        </p>
      </div>
    </div>
  )
}
