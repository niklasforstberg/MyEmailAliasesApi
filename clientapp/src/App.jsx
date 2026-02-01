import { useContext, useState, useEffect } from 'react'
import { AuthContext } from './context/AuthContext'
import Login from './components/Login'
import ForgotPassword from './components/ForgotPassword'
import ResetPassword from './components/ResetPassword'
import AliasesList from './components/AliasesList'
import './App.css'

// Mail icon for header
function MailIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <rect x="2" y="4" width="20" height="16" rx="2" />
      <path d="M22 7l-10 7L2 7" />
    </svg>
  )
}

// Logout icon
function LogoutIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
      <polyline points="16 17 21 12 16 7" />
      <line x1="21" y1="12" x2="9" y2="12" />
    </svg>
  )
}

function App() {
  const { isAuthenticated, loading } = useContext(AuthContext)
  const [view, setView] = useState('login')
  const [resetToken, setResetToken] = useState(null)

  useEffect(() => {
    const params = new URLSearchParams(window.location.search)
    const token = params.get('token')
    if (token) {
      setResetToken(token)
      setView('resetPassword')
      window.history.replaceState({}, document.title, window.location.pathname)
    }
  }, [])

  if (loading) {
    return (
      <div className="app">
        <div className="loading-screen">
          <div className="loading-spinner" />
          <div className="loading-text">Loading...</div>
        </div>
      </div>
    )
  }

  const renderAuthView = () => {
    switch (view) {
      case 'forgotPassword':
        return <ForgotPassword onBackToLogin={() => setView('login')} />
      case 'resetPassword':
        return <ResetPassword token={resetToken} onSuccess={() => setView('login')} />
      case 'login':
      default:
        return <Login onForgotPassword={() => setView('forgotPassword')} />
    }
  }

  return (
    <div className="app">
      <header className="app-header">
        <div className="header-brand">
          <div className="header-logo">
            <MailIcon />
          </div>
          <div className="header-text">
            <h1>Email Aliases</h1>
          </div>
        </div>

        {isAuthenticated && (
          <button
            onClick={() => {
              localStorage.removeItem('token')
              window.location.reload()
            }}
            className="logout-btn"
          >
            <LogoutIcon />
            <span>Logout</span>
          </button>
        )}
      </header>

      <main className="app-main">
        {!isAuthenticated ? renderAuthView() : <AliasesList />}
      </main>
    </div>
  )
}

export default App
