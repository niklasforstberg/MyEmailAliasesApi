import { useState, useContext } from 'react'
import { AuthContext } from '../context/AuthContext'
import './Login.css'

// Snowflake icon for decoration
function SnowflakeIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <line x1="12" y1="2" x2="12" y2="22" />
      <path d="M20 12h-8" />
      <path d="M4 12h8" />
      <path d="M6.3 6.3l11.4 11.4" />
      <path d="M17.7 6.3L6.3 17.7" />
      <path d="M12 2l3 4" />
      <path d="M12 2l-3 4" />
      <path d="M12 22l3-4" />
      <path d="M12 22l-3-4" />
      <path d="M2 12l4 3" />
      <path d="M2 12l4-3" />
      <path d="M22 12l-4 3" />
      <path d="M22 12l-4-3" />
    </svg>
  )
}

function Login({ onForgotPassword }) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { login } = useContext(AuthContext)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    const result = await login(email, password)

    if (!result.success) {
      setError(result.error || 'Login failed. Please check your credentials.')
    }

    setLoading(false)
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="card-decoration">
          <div className="decoration-line" />
          <div className="decoration-icon">
            <SnowflakeIcon />
          </div>
          <div className="decoration-line" />
        </div>

        <h2>Welcome Back</h2>
        <p className="login-card-subtitle">Sign in to your mountain lodge</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="text"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
              placeholder="Enter your email"
              autoComplete="email"
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={loading}
              placeholder="Enter your password"
              autoComplete="current-password"
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" disabled={loading} className="submit-btn">
            {loading ? 'Signing in...' : 'Sign In'}
          </button>

          {onForgotPassword && (
            <div className="form-footer">
              <button
                type="button"
                onClick={onForgotPassword}
                className="link-btn"
              >
                Forgot Password?
              </button>
            </div>
          )}
        </form>
      </div>
    </div>
  )
}

export default Login
