import { useState } from 'react'
import { api } from '../services/api'
import './Login.css'

// Key icon for password reset
function KeyIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M21 2l-2 2m-7.61 7.61a5.5 5.5 0 1 1-7.778 7.778 5.5 5.5 0 0 1 7.777-7.777zm0 0L15.5 7.5m0 0l3 3L22 7l-3-3m-3.5 3.5L19 4" />
    </svg>
  )
}

function ForgotPassword({ onBackToLogin }) {
  const [email, setEmail] = useState('')
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSuccess(false)
    setLoading(true)

    try {
      await api.post('/auth/forgot-password', { email })
      setSuccess(true)
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to send reset email. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="card-decoration">
          <div className="decoration-line" />
          <div className="decoration-icon">
            <KeyIcon />
          </div>
          <div className="decoration-line" />
        </div>

        <h2>Forgot Password</h2>
        <p className="login-card-subtitle">We'll help you get back in</p>

        {success ? (
          <div>
            <div className="success-message">
              If an account with that email exists, a password reset link has been sent.
            </div>
            <button
              onClick={onBackToLogin}
              className="submit-btn"
            >
              Back to Login
            </button>
          </div>
        ) : (
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="email">Email Address</label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                disabled={loading}
                placeholder="Enter your email"
                required
                autoComplete="email"
              />
            </div>

            {error && <div className="error-message">{error}</div>}

            <button type="submit" disabled={loading} className="submit-btn">
              {loading ? 'Sending...' : 'Send Reset Link'}
            </button>

            <div className="form-footer">
              <button
                type="button"
                onClick={onBackToLogin}
                className="link-btn"
              >
                Back to Login
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}

export default ForgotPassword
