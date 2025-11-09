import { useContext } from 'react'
import { AuthContext } from './context/AuthContext'
import Login from './components/Login'
import AliasesList from './components/AliasesList'
import './App.css'

function App() {
  const { isAuthenticated, loading } = useContext(AuthContext)

  if (loading) {
    return (
      <div className="app">
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
          <div>Loading...</div>
        </div>
      </div>
    )
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
        {!isAuthenticated ? <Login /> : <AliasesList />}
      </main>
    </div>
  )
}

export default App

