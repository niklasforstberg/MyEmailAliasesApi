import { useContext, useState, useEffect } from 'react'
import { AuthContext } from './context/AuthContext'
import Login from './components/Login'
import ForgotPassword from './components/ForgotPassword'
import ResetPassword from './components/ResetPassword'
import AliasesList from './components/AliasesList'
import './App.css'

function App() {
  const { isAuthenticated, loading } = useContext(AuthContext)
  const [view, setView] = useState('login')
  const [resetToken, setResetToken] = useState(null)

  useEffect(() => {
    // Check for reset token in URL query params
    const params = new URLSearchParams(window.location.search)
    const token = params.get('token')
    if (token) {
      setResetToken(token)
      setView('resetPassword')
      // Clean up URL
      window.history.replaceState({}, document.title, window.location.pathname)
    }
  }, [])

  if (loading) {
    return (
      <div className="app">
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
          <div>Loading...</div>
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
        <h1>Email Aliases Manager</h1>
        {isAuthenticated && (
          <button 
            onClick={() => {
              localStorage.removeItem('token')
              window.location.reload()
            }}
            className="logout-btn"
          >
            Logout
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

